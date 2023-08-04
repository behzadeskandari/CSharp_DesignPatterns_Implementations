using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.IO.Pipes;
using System.IO.Pipelines;
using SmtpServer.Net;
using System.Net.Security;
using SmtpServer.Mail;
using System.Collections.ObjectModel;
using System.Collections;

namespace Automation.Common.PortalConfig
{

    //singleton Example On Portal Configurations 
    public class ERPConfigurations
    {
        private static Lazy<ERPConfigurations> instance;
        private string DatabaseConncetionString;
        private int _maxConcurrentUser;
        private SmtpServer _smptServer;
        ISmtpServerOptions _SmtpServerOptions;
        IServiceProvider _ServiceProvider;
        public ERPConfigurations()
        {
            DatabaseConncetionString = "Data Source=erp-database;User=sa;Password=pas123";
            _maxConcurrentUser = 100;
            _smptServer = new SmtpServer(_SmtpServerOptions, _ServiceProvider);
        }

        /// <summary>
        /// old Way of Instansiations 
        /// </summary>
        //public ERPConfigurations(string connncetionString,int maxConcurrentUser,SmtpServer smptserver)
        //{
        //    DatabaseConncetionString = connncetionString,
        //    _maxConcurrentUser = maxConcurrentUser,
        //    _smptServer = smptserver
        //}

        public static Lazy<ERPConfigurations> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Lazy<ERPConfigurations>();
                }

                return instance;

            }
        }

        public string GetDatabaseConnectionString()
        {
            return DatabaseConncetionString;
        }

        public int GetMaxConcurrentUser()
        {
            return _maxConcurrentUser;
        }

        public SmtpServer GetSmptServer()
        {
            return _smptServer;
        }
        //other methods for ERP Configuration management
    }

    public class ERPClient
    {
        public void PrintConfigurationDetails()
        {
            ERPConfigurations erpConfig = ERPConfigurations.Instance.Value;

            Console.WriteLine("db connction String " + erpConfig.GetDatabaseConnectionString());
            Console.WriteLine("db max concurrent User " + erpConfig.GetMaxConcurrentUser());
            Console.WriteLine("db smtpServer " + erpConfig.GetSmptServer());
        }
    }




    /// <summary>
    /// implementation 
    /// </summary>
    /// 
    public class Program
    {
        static void Main()
        {
            ERPClient client = new ERPClient();
            client.PrintConfigurationDetails();
        }
    }


    public class SmtpServer
    {
        /// <summary>
        /// Raised when a session has been created.
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionCreated;

        /// <summary>
        /// Raised when a session has completed.
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionCompleted;

        /// <summary>
        /// Raised when a session has faulted.
        /// </summary>
        public event EventHandler<SessionFaultedEventArgs> SessionFaulted;

        /// <summary>
        /// Raised when a session has been cancelled through the cancellation token.
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionCancelled;

        readonly ISmtpServerOptions _options;
        readonly IServiceProvider _serviceProvider;
        readonly IEndpointListenerFactory _endpointListenerFactory;
        readonly SmtpSessionManager _sessions;
        readonly CancellationTokenSource _shutdownTokenSource = new CancellationTokenSource();
        readonly TaskCompletionSource<bool> _shutdownTask = new TaskCompletionSource<bool>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The SMTP server options.</param>
        /// <param name="serviceProvider">The service provider to use when resolving services.</param>
        public SmtpServer(ISmtpServerOptions options, IServiceProvider serviceProvider)
        {
            _options = options;
            _serviceProvider = serviceProvider;
            _sessions = new SmtpSessionManager(this);
            var t  = serviceProvider.GetService(serviceType: EndpointListenerFactory.EndPointType.GetType());
            _endpointListenerFactory = t as IEndpointListenerFactory;
        }

        /// <summary>
        /// Raises the SessionCreated Event.
        /// </summary>
        /// <param name="args">The event data.</param>
        protected internal virtual void OnSessionCreated(SessionEventArgs args)
        {
            SessionCreated?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the SessionCompleted Event.
        /// </summary>
        /// <param name="args">The event data.</param>
        protected internal virtual void OnSessionCompleted(SessionEventArgs args)
        {
            SessionCompleted?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the SessionCompleted Event.
        /// </summary>
        /// <param name="args">The event data.</param>
        protected internal virtual void OnSessionFaulted(SessionFaultedEventArgs args)
        {
            SessionFaulted?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the SessionCancelled Event.
        /// </summary>
        /// <param name="args">The event data.</param>
        protected internal virtual void OnSessionCancelled(SessionEventArgs args)
        {
            SessionCancelled?.Invoke(this, args);
        }

        /// <summary>
        /// Starts the SMTP server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which performs the operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var tasks = _options.Endpoints.Select(e => ListenAsync(e, cancellationToken));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            _shutdownTask.TrySetResult(true);

            await _sessions.WaitAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Shutdown the server and allow any active sessions to finish.
        /// </summary>
        public void Shutdown()
        {
            _shutdownTokenSource.Cancel();
        }

        /// <summary>
        /// Listen for SMTP traffic on the given endpoint.
        /// </summary>
        /// <param name="endpointDefinition">The definition of the endpoint to listen on.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which performs the operation.</returns>
        async Task ListenAsync(IEndpointDefinition endpointDefinition, CancellationToken cancellationToken)
        {
            // The listener can be stopped either by the caller cancelling the CancellationToken used when starting the server, or when calling
            // the shutdown method. The Shutdown method will stop the listeners and allow any active sessions to finish gracefully.
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_shutdownTokenSource.Token, cancellationToken);

            using var endpointListener = _endpointListenerFactory.CreateListener(endpointDefinition);

            while (cancellationTokenSource.Token.IsCancellationRequested == false)
            {
                var sessionContext = new SmtpSessionContext(_serviceProvider, _options, endpointDefinition);

                try
                {
                    // wait for a client connection
                    sessionContext.Pipe = await endpointListener.GetPipeAsync(sessionContext, cancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    OnSessionFaulted(new SessionFaultedEventArgs(sessionContext, ex));
                    continue;
                }

                if (sessionContext.Pipe != null)
                {
                    _sessions.Run(sessionContext, cancellationTokenSource.Token);
                }
            }
        }

        /// <summary>
        /// The task that completes when the server has shutdown and stopped accepting new sessions.
        /// </summary>
        public Task ShutdownTask => _shutdownTask.Task;
    }



    public interface ISmtpServerOptions
    {
        /// <summary>
        /// Gets the maximum size of a message.
        /// </summary>
        int MaxMessageSize { get; }

        /// <summary>
        /// The maximum number of retries before quitting the session.
        /// </summary>
        int MaxRetryCount { get; }

        /// <summary>
        /// The maximum number of authentication attempts.
        /// </summary>
        int MaxAuthenticationAttempts { get; }

        /// <summary>
        /// Gets the SMTP server name.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets the collection of endpoints to listen on.
        /// </summary>
        IReadOnlyList<IEndpointDefinition> Endpoints { get; }

        /// <summary>
        /// The timeout to use when waiting for a command from the client.
        /// </summary>
        TimeSpan CommandWaitTimeout { get; }

        /// <summary>
        /// The size of the buffer that is read from each call to the underlying network client.
        /// </summary>
        int NetworkBufferSize { get; }
    }


    public class SessionEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The session context.</param>
        public SessionEventArgs(ISessionContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Returns the session context.
        /// </summary>
        public ISessionContext Context { get; }
    }



    public interface IEndpointDefinition
    {
        /// <summary>
        /// The IP endpoint to listen on.
        /// </summary>
        IPEndPoint Endpoint { get; }

        /// <summary>
        /// Indicates whether the endpoint is secure by default.
        /// </summary>
        bool IsSecure { get; }

        /// <summary>
        /// Gets a value indicating whether the client must authenticate in order to proceed.
        /// </summary>
        bool AuthenticationRequired { get; }

        /// <summary>
        /// Gets a value indicating whether authentication should be allowed on an unsecure session.
        /// </summary>
        bool AllowUnsecureAuthentication { get; }

        /// <summary>
        /// The timeout on each individual buffer read.
        /// </summary>
        TimeSpan ReadTimeout { get; }

        /// <summary>
        /// Gets the Server Certificate to use when starting a TLS session.
        /// </summary>
        X509Certificate ServerCertificate { get; }

        /// <summary>
        /// The supported SSL protocols.
        /// </summary>
        SslProtocols SupportedSslProtocols { get; }
    }



    public class SessionFaultedEventArgs : SessionEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The session context.</param>
        /// <param name="exception">The exception that occured</param>
        public SessionFaultedEventArgs(ISessionContext context, Exception exception) : base(context)
        {
            Exception = exception;
        }

        /// <summary>
        /// Returns the exception.
        /// </summary>
        public Exception Exception { get; }
    }


    public interface IEndpointListenerFactory
    {
        /// <summary>
        /// Create an instance of an endpoint listener for the specified endpoint definition.
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition to create the listener for.</param>
        /// <returns>The endpoint listener for the specified endpoint definition.</returns>
        IEndpointListener CreateListener(IEndpointDefinition endpointDefinition);
    }



    internal sealed class SmtpSessionManager
    {
        readonly SmtpServer _smtpServer;
        readonly HashSet<SmtpSessionHandle> _sessions = new HashSet<SmtpSessionHandle>();
        readonly object _sessionsLock = new object();

        internal SmtpSessionManager(SmtpServer smtpServer)
        {
            _smtpServer = smtpServer;
        }

        internal void Run(SmtpSessionContext sessionContext, CancellationToken cancellationToken)
        {
            var handle = new SmtpSessionHandle(new SmtpSession(sessionContext), sessionContext);
            Add(handle);

            handle.CompletionTask = RunAsync(handle, cancellationToken);

            // ReSharper disable once MethodSupportsCancellation
            handle.CompletionTask.ContinueWith(
                task =>
                {
                    Remove(handle);
                });
        }

        async Task RunAsync(SmtpSessionHandle handle, CancellationToken cancellationToken)
        {
            try
            {
                await UpgradeAsync(handle, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                _smtpServer.OnSessionCreated(new SessionEventArgs(handle.SessionContext));

                await handle.Session.RunAsync(cancellationToken);

                _smtpServer.OnSessionCompleted(new SessionEventArgs(handle.SessionContext));
            }
            catch (OperationCanceledException)
            {
                _smtpServer.OnSessionCancelled(new SessionEventArgs(handle.SessionContext));
            }
            catch (Exception ex)
            {
                _smtpServer.OnSessionFaulted(new SessionFaultedEventArgs(handle.SessionContext, ex));
            }
            finally
            {
                await handle.SessionContext.Pipe.Input.CompleteAsync();

                handle.SessionContext.Pipe.Dispose();
            }
        }

        async Task UpgradeAsync(SmtpSessionHandle handle, CancellationToken cancellationToken)
        {
            var endpoint = handle.SessionContext.EndpointDefinition;

            if (endpoint.IsSecure && endpoint.ServerCertificate != null)
            {
                await handle.SessionContext.Pipe.UpgradeAsync(endpoint.ServerCertificate, endpoint.SupportedSslProtocols, cancellationToken).ConfigureAwait(false);
            }
        }

        internal Task WaitAsync()
        {
            IReadOnlyList<Task> tasks;

            lock (_sessionsLock)
            {
                tasks = _sessions.Select(session => session.CompletionTask).ToList();
            }

            return Task.WhenAll(tasks);
        }

        void Add(SmtpSessionHandle handle)
        {
            lock (_sessionsLock)
            {
                _sessions.Add(handle);
            }
        }

        void Remove(SmtpSessionHandle handle)
        {
            lock (_sessionsLock)
            {
                _sessions.Remove(handle);
            }
        }


    }

    class SmtpSessionHandle
    {
        public SmtpSessionHandle(SmtpSession session, SmtpSessionContext sessionContext)
        {
            Session = session;
            SessionContext = sessionContext;
        }

        public SmtpSession Session { get; }

        public SmtpSessionContext SessionContext { get; }

        public Task CompletionTask { get; set; }
    }


    public sealed class SmtpCommandEventArgs : SessionEventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The session context.</param>
        /// <param name="command">The command for the event.</param>
        public SmtpCommandEventArgs(ISessionContext context, SmtpCommand command) : base(context)
        {
            Command = command;
        }

        /// <summary>
        /// The command for the event.
        /// </summary>
        public SmtpCommand Command { get; }
    }

    public abstract class SmtpCommand
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        protected SmtpCommand(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="context">The execution context to operate on.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns true if the command executed successfully such that the transition to the next state should occurr, false 
        /// if the current state is to be maintained.</returns>
        internal abstract Task<bool> ExecuteAsync(SmtpSessionContext context, CancellationToken cancellationToken);

        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Name { get; }
    }

    internal sealed class SmtpSessionContext : ISessionContext
    {
        /// <summary>
        /// Fired when a command is about to execute.
        /// </summary>
        public event EventHandler<SmtpCommandEventArgs> CommandExecuting;

        /// <summary>
        /// Fired when a command has finished executing.
        /// </summary>
        public event EventHandler<SmtpCommandEventArgs> CommandExecuted;

        /// <summary>
        /// Fired when the session has been authenticated.
        /// </summary>
        public event EventHandler<EventArgs> SessionAuthenticated;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceProvider">The service provider instance.</param>
        /// <param name="options">The server options.</param>
        /// <param name="endpointDefinition">The endpoint definition.</param>
        internal SmtpSessionContext(IServiceProvider serviceProvider, ISmtpServerOptions options, IEndpointDefinition endpointDefinition)
        {
            ServiceProvider = serviceProvider;
            ServerOptions = options;
            EndpointDefinition = endpointDefinition;
            Transaction = new SmtpMessageTransaction();
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Raise the command executing event.
        /// </summary>
        /// <param name="command">The command that is executing.</param>
        internal void RaiseCommandExecuting(SmtpCommand command)
        {
            CommandExecuting?.Invoke(this, new SmtpCommandEventArgs(this, command));
        }

        /// <summary>
        /// Raise the command executed event.
        /// </summary>
        /// <param name="command">The command that was executed.</param>
        internal void RaiseCommandExecuted(SmtpCommand command)
        {
            CommandExecuted?.Invoke(this, new SmtpCommandEventArgs(this, command));
        }

        /// <summary>
        /// Raise the session authenticated event.
        /// </summary>
        internal void RaiseSessionAuthenticated()
        {
            SessionAuthenticated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The service provider instance. 
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the options that the server was created with.
        /// </summary>
        public ISmtpServerOptions ServerOptions { get; }

        /// <summary>
        /// Gets the endpoint definition.
        /// </summary>
        public IEndpointDefinition EndpointDefinition { get; }

        /// <summary>
        /// Gets the pipeline to read from and write to.
        /// </summary>
        public ISecurableDuplexPipe Pipe { get; internal set; }

        /// <summary>
        /// Gets the current transaction.
        /// </summary>
        public SmtpMessageTransaction Transaction { get; }

        /// <summary>
        /// Returns the authentication context.
        /// </summary>
        public AuthenticationContext Authentication { get; internal set; } = AuthenticationContext.Unauthenticated;

        /// <summary>
        /// Returns the number of athentication attempts.
        /// </summary>
        public int AuthenticationAttempts { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether a quit has been requested.
        /// </summary>
        public bool IsQuitRequested { get; internal set; }

        /// <summary>
        /// Returns a set of propeties for the current session.
        /// </summary>
        public IDictionary<string, object> Properties { get; }
    }


    public interface ISecurableDuplexPipe : IDuplexPipe, IDisposable
    {
        /// <summary>
        /// Upgrade to a secure pipeline.
        /// </summary>
        /// <param name="certificate">The X509Certificate used to authenticate the server.</param>
        /// <param name="protocols">The value that represents the protocol used for authentication.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that asynchronously performs the operation.</returns>
        Task UpgradeAsync(X509Certificate certificate, SslProtocols protocols, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a value indicating whether or not the current pipeline is secure.
        /// </summary>
        bool IsSecure { get; }
    }


    public class EndpointListenerFactory : IEndpointListenerFactory
    {
        internal static readonly IEndpointListenerFactory Default = new EndpointListenerFactory();

        internal static readonly Type EndPointType = typeof(EndpointListenerFactory); 


        /// <summary>
        /// Raised when an endpoint has been started.
        /// </summary>
        public event EventHandler<EndpointEventArgs> EndpointStarted;

        /// <summary>
        /// Raised when an endpoint has been stopped.
        /// </summary>
        public event EventHandler<EndpointEventArgs> EndpointStopped;

        /// <summary>
        /// Create an instance of an endpoint listener for the specified endpoint definition.
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition to create the listener for.</param>
        /// <returns>The endpoint listener for the specified endpoint definition.</returns>
        public virtual IEndpointListener CreateListener(IEndpointDefinition endpointDefinition)
        {
            var tcpListener = new TcpListener(endpointDefinition.Endpoint);
            tcpListener.Start();

            var endpointEventArgs = new EndpointEventArgs(endpointDefinition, tcpListener.LocalEndpoint);
            OnEndpointStarted(endpointEventArgs);

            return new EndpointListener(endpointDefinition, tcpListener, () => OnEndpointStopped(endpointEventArgs));
        }

        /// <summary>
        /// Raises the EndPointStarted Event.
        /// </summary>
        /// <param name="args">The event data.</param>
        protected virtual void OnEndpointStarted(EndpointEventArgs args)
        {
            EndpointStarted?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the EndPointStopped Event.
        /// </summary>
        /// <param name="args">The event data.</param>
        protected virtual void OnEndpointStopped(EndpointEventArgs args)
        {
            EndpointStopped?.Invoke(this, args);
        }
    }

    public sealed class EndpointEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition.</param>
        /// <param name="localEndPoint">The locally bound endpoint.</param>
        public EndpointEventArgs(IEndpointDefinition endpointDefinition, EndPoint localEndPoint)
        {
            EndpointDefinition = endpointDefinition;
            LocalEndPoint = localEndPoint;
        }

        /// <summary>
        /// Returns the endpoint definition.
        /// </summary>
        public IEndpointDefinition EndpointDefinition { get; }

        /// <summary>
        /// Returns the locally bound endpoint
        /// </summary>
        public EndPoint LocalEndPoint { get; }
    }
    public sealed class EndpointListener : IEndpointListener
    {
        public const string LocalEndPointKey = "EndpointListener:LocalEndPoint";
        public const string RemoteEndPointKey = "EndpointListener:RemoteEndPoint";

        readonly IEndpointDefinition _endpointDefinition;
        readonly TcpListener _tcpListener;
        readonly Action _disposeAction;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="endpointDefinition">The endpoint definition to create the listener for.</param>
        /// <param name="tcpListener">The TCP listener for the endpoint.</param>
        /// <param name="disposeAction">The action to execute when the listener has been disposed.</param>
        internal EndpointListener(IEndpointDefinition endpointDefinition, TcpListener tcpListener, Action disposeAction)
        {
            _endpointDefinition = endpointDefinition;
            _tcpListener = tcpListener;
            _disposeAction = disposeAction;
        }

        /// <summary>
        /// Returns a securable pipe to the endpoint.
        /// </summary>
        /// <param name="context">The session context that the pipe is being created for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The securable pipe from the endpoint.</returns>
        public async Task<ISecurableDuplexPipe> GetPipeAsync(ISessionContext context, CancellationToken cancellationToken)
        {
            var tcpClient = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            context.Properties.Add(LocalEndPointKey, _tcpListener.LocalEndpoint);
            context.Properties.Add(RemoteEndPointKey, tcpClient.Client.RemoteEndPoint);

            var stream = tcpClient.GetStream();
            stream.ReadTimeout = (int)_endpointDefinition.ReadTimeout.TotalMilliseconds;

            return new SecurableDuplexPipe(stream, () =>
            {
                tcpClient.Close();
                tcpClient.Dispose();
            });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _tcpListener.Stop();
            _disposeAction();
        }

        public Task<global::SmtpServer.IO.ISecurableDuplexPipe> GetPipeAsync(global::SmtpServer.ISessionContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


    internal sealed class SecurableDuplexPipe : ISecurableDuplexPipe
    {
        readonly Action _disposeAction;
        Stream _stream;
        bool _disposed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream that the pipe is reading and writing to.</param>
        /// <param name="disposeAction">The action to execute when the stream has been disposed.</param>
        internal SecurableDuplexPipe(Stream stream, Action disposeAction)
        {
            _stream = stream;
            _disposeAction = disposeAction;

            Input = PipeReader.Create(_stream);
            Output = PipeWriter.Create(_stream);
        }

        /// <summary>
        /// Upgrade to a secure pipeline.
        /// </summary>
        /// <param name="certificate">The X509Certificate used to authenticate the server.</param>
        /// <param name="protocols">The value that represents the protocol used for authentication.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that asynchronously performs the operation.</returns>
        public async Task UpgradeAsync(X509Certificate certificate, SslProtocols protocols, CancellationToken cancellationToken = default)
        {
            var stream = new SslStream(_stream, true);

            await stream.AuthenticateAsServerAsync(certificate, false, protocols, true).ConfigureAwait(false);

            _stream = stream;

            Input = PipeReader.Create(_stream);
            Output = PipeWriter.Create(_stream);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the stream and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    _disposeAction();
                    _stream = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Gets the <see cref="T:System.IO.Pipelines.PipeReader" /> half of the duplex pipe.
        /// </summary>
        public PipeReader Input { get; private set; }

        /// <summary>
        /// Gets the <see cref="T:System.IO.Pipelines.PipeWriter" /> half of the duplex pipe.
        /// </summary>
        public PipeWriter Output { get; private set; }

        /// <summary>
        /// Returns a value indicating whether or not the current pipeline is secure.
        /// </summary>
        public bool IsSecure => _stream is SslStream;
    }


    public sealed class AuthenticationContext
    {
        public static readonly AuthenticationContext Unauthenticated = new AuthenticationContext();

        /// <summary>
        /// Constructor.
        /// </summary>
        public AuthenticationContext()
        {
            IsAuthenticated = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The name of the user that was authenticated.</param>
        public AuthenticationContext(string user)
        {
            User = user;
            IsAuthenticated = true;
        }

        /// <summary>
        /// The name of the user that was authenticated.
        /// </summary>
        public string User { get; }

        /// <summary>
        /// Returns a value indicating whether or nor the current session is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; }
    }

    public interface IMessageTransaction
    {
        /// <summary>
        /// Gets or sets the mailbox that is sending the message.
        /// </summary>
        IMailbox From { get; set; }

        /// <summary>
        /// Gets the collection of mailboxes that the message is to be delivered to.
        /// </summary>
        IList<IMailbox> To { get; }

        /// <summary>
        /// The list of parameters that were supplied by the client.
        /// </summary>
        IReadOnlyDictionary<string, string> Parameters { get; }
    }

    internal sealed class SmtpMessageTransaction : IMessageTransaction
    {
        /// <summary>
        /// Reset the current transaction.
        /// </summary>
        public void Reset()
        {
            From = null;
            To = new Collection<IMailbox>();
            Parameters = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        /// <summary>
        /// Gets or sets the mailbox that is sending the message.
        /// </summary>
        public IMailbox From { get; set; }

        /// <summary>
        /// Gets or sets the collection of mailboxes that the message is to be delivered to.
        /// </summary>
        public IList<IMailbox> To { get; set; } = new Collection<IMailbox>();

        /// <summary>
        /// The list of parameters that were supplied by the client.
        /// </summary>
        public IReadOnlyDictionary<string, string> Parameters { get; set; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
    }

    public interface ISessionContext
    {
        /// <summary>
        /// Fired when a command is about to execute.
        /// </summary>
        event EventHandler<SmtpCommandEventArgs> CommandExecuting;

        /// <summary>
        /// Fired when a command has finished executing.
        /// </summary>
        event EventHandler<SmtpCommandEventArgs> CommandExecuted;

        /// <summary>
        /// Fired when the session has been authenticated.
        /// </summary>
        event EventHandler<EventArgs> SessionAuthenticated;

        /// <summary>
        /// The service provider instance. 
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the options that the server was created with.
        /// </summary>
        ISmtpServerOptions ServerOptions { get; }

        /// <summary>
        /// Gets the endpoint definition.
        /// </summary>
        IEndpointDefinition EndpointDefinition { get; }

        /// <summary>
        /// Gets the pipeline to read from and write to.
        /// </summary>
        ISecurableDuplexPipe Pipe { get; }

        /// <summary>
        /// Returns the authentication context.
        /// </summary>
        AuthenticationContext Authentication { get; }

        /// <summary>
        /// Returns a set of propeties for the current session.
        /// </summary>
        IDictionary<string, object> Properties { get; }
    }

    internal sealed class SmtpSession
    {
        readonly SmtpStateMachine _stateMachine;
        readonly SmtpSessionContext _context;
        readonly ISmtpCommandFactory _commandFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The session context.</param>
        internal SmtpSession(SmtpSessionContext context)
        {
            _context = context;
            _stateMachine = new SmtpStateMachine(_context);
            _commandFactory = context.ServiceProvider.GetServiceOrDefault<ISmtpCommandFactory>(new SmtpCommandFactory());
        }

        /// <summary>
        /// Handles the SMTP session.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which performs the operation.</returns>
        internal async Task RunAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await OutputGreetingAsync(cancellationToken).ConfigureAwait(false);

            await ExecuteAsync(_context, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute the command handler against the specified session context.
        /// </summary>
        /// <param name="context">The session context to execute the command handler against.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the execution.</returns>
        async Task ExecuteAsync(SmtpSessionContext context, CancellationToken cancellationToken)
        {
            var retries = _context.ServerOptions.MaxRetryCount;

            while (retries-- > 0 && context.IsQuitRequested == false && cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    var command = await ReadCommandAsync(context, cancellationToken).ConfigureAwait(false);

                    if (command == null)
                    {
                        return;
                    }

                    if (_stateMachine.TryAccept(command, out var errorResponse) == false)
                    {
                        throw new SmtpResponseException(errorResponse);
                    }

                    if (await ExecuteAsync(command, context, cancellationToken).ConfigureAwait(false))
                    {
                        _stateMachine.Transition(context);
                    }

                    retries = _context.ServerOptions.MaxRetryCount;
                }
                catch (SmtpResponseException responseException) when (responseException.IsQuitRequested)
                {
                    await context.Pipe.Output.WriteAsync(responseException.Response, cancellationToken).ConfigureAwait(false);

                    context.IsQuitRequested = true;
                }
                catch (SmtpResponseException responseException)
                {
                    var response = CreateErrorResponse(responseException.Response, retries);

                    await context.Pipe.Output.WriteReplyAsync(response, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    await context.Pipe.Output.WriteReplyAsync(new SmtpResponse(SmtpReplyCode.ServiceClosingTransmissionChannel, "The session has be cancelled."), CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        async ValueTask<SmtpCommand> ReadCommandAsync(ISessionContext context, CancellationToken cancellationToken)
        {
            var timeout = new CancellationTokenSource(context.ServerOptions.CommandWaitTimeout);

            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken);

            try
            {
                SmtpCommand command = null;

                await context.Pipe.Input.ReadAsync(
                    //buffer =>
                    //{
                    //    var parser = new SmtpParser(_commandFactory);

                    //    if (parser.TryMake(ref buffer, out command, out var errorResponse) == false)
                    //    {
                    //        throw new SmtpResponseException(errorResponse);
                    //    }

                    //    return Task.CompletedTask;
                    //},
                    //cancellationTokenSource.Token
                    ).ConfigureAwait(false);

                return command;
            }
            catch (OperationCanceledException)
            {
                if (timeout.IsCancellationRequested)
                {
                    throw new SmtpResponseException(new SmtpResponse(SmtpReplyCode.ServiceClosingTransmissionChannel, "Timeout while waiting for input."), true);
                }

                throw new SmtpResponseException(new SmtpResponse(SmtpReplyCode.ServiceClosingTransmissionChannel, "The session has be cancelled."), true);
            }
            finally
            {
                timeout.Dispose();
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// Create an error response.
        /// </summary>
        /// <param name="response">The original response to wrap with the error message information.</param>
        /// <param name="retries">The number of retries remaining before the session is terminated.</param>
        /// <returns>The response that wraps the original response with the additional error information.</returns>
        static SmtpResponse CreateErrorResponse(SmtpResponse response, int retries)
        {
            return new SmtpResponse(response.ReplyCode, $"{response.Message}, {retries} retry(ies) remaining.");
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="context">The execution context to operate on.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which asynchronously performs the execution.</returns>
        static async Task<bool> ExecuteAsync(SmtpCommand command, SmtpSessionContext context, CancellationToken cancellationToken)
        {
            context.RaiseCommandExecuting(command);

            var result = await command.ExecuteAsync(context, cancellationToken);

            context.RaiseCommandExecuted(command);

            return result;
        }

        /// <summary>
        /// Output the greeting.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task which performs the operation.</returns>
        ValueTask<FlushResult> OutputGreetingAsync(CancellationToken cancellationToken)
        {
            var version = typeof(SmtpSession).GetType().Assembly.GetName().Version;

            _context.Pipe.Output.Complete(); ///.WriteLine($"220 {_context.ServerOptions.ServerName} v{version} ESMTP ready");

            return _context.Pipe.Output.FlushAsync(cancellationToken);
        }
    }

    internal sealed class SmtpStateMachine
    {
        readonly SmtpSessionContext _context;
        SmtpState _state;
        SmtpStateTransition _transition;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The SMTP server session context.</param>
        internal SmtpStateMachine(SmtpSessionContext context)
        {
            _state = SmtpStateTable.Shared[SmtpStateId.Initialized];
            _context = context;
        }

        /// <summary>
        /// Try to accept the command given the current state.
        /// </summary>
        /// <param name="command">The command to accept.</param>
        /// <param name="errorResponse">The error response to display if the command was not accepted.</param>
        /// <returns>true if the command could be accepted, false if not.</returns>
        public bool TryAccept(SmtpCommand command, out SmtpResponse errorResponse)
        {
            errorResponse = null;

            if (_state.Transitions.TryGetValue(command.Name, out var transition) == false || transition.CanAccept(_context) == false)
            {
                var commands = _state.Transitions.Where(t => t.Value.CanAccept(_context)).Select(t => t.Key);

                errorResponse = new SmtpResponse(SmtpReplyCode.SyntaxError, $"expected {string.Join("/", commands)}");
                return false;
            }

            _transition = transition;
            return true;
        }

        /// <summary>
        /// Accept the state and transition to the new state.
        /// </summary>
        /// <param name="context">The session context to use for accepting session based transitions.</param>
        public void Transition(SmtpSessionContext context)
        {
            _state = SmtpStateTable.Shared[_transition.Transition(context)];
        }
    }

    public class SmtpResponse
    {
        public static readonly SmtpResponse Ok = new SmtpResponse(SmtpReplyCode.Ok, "Ok");
        public static readonly SmtpResponse ServiceReady = new SmtpResponse(SmtpReplyCode.ServiceReady, "ready when you are");
        public static readonly SmtpResponse MailboxUnavailable = new SmtpResponse(SmtpReplyCode.MailboxUnavailable, "mailbox unavailable");
        public static readonly SmtpResponse MailboxNameNotAllowed = new SmtpResponse(SmtpReplyCode.MailboxNameNotAllowed, "mailbox name not allowed");
        public static readonly SmtpResponse ServiceClosingTransmissionChannel = new SmtpResponse(SmtpReplyCode.ServiceClosingTransmissionChannel, "bye");
        public static readonly SmtpResponse SyntaxError = new SmtpResponse(SmtpReplyCode.SyntaxError, "syntax error");
        public static readonly SmtpResponse SizeLimitExceeded = new SmtpResponse(SmtpReplyCode.SizeLimitExceeded, "size limit exceeded");
        public static readonly SmtpResponse NoValidRecipientsGiven = new SmtpResponse(SmtpReplyCode.TransactionFailed, "no valid recipients given");
        public static readonly SmtpResponse AuthenticationFailed = new SmtpResponse(SmtpReplyCode.AuthenticationFailed, "authentication failed");
        public static readonly SmtpResponse AuthenticationSuccessful = new SmtpResponse(SmtpReplyCode.AuthenticationSuccessful, "go ahead");
        public static readonly SmtpResponse TransactionFailed = new SmtpResponse(SmtpReplyCode.TransactionFailed);
        public static readonly SmtpResponse BadSequence = new SmtpResponse(SmtpReplyCode.BadSequence, "bad sequence of commands");
        public static readonly SmtpResponse AuthenticationRequired = new SmtpResponse(SmtpReplyCode.AuthenticationRequired, "authentication required");

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="replyCode">The reply code.</param>
        /// <param name="message">The reply message.</param>
        public SmtpResponse(SmtpReplyCode replyCode, string message = null)
        {
            ReplyCode = replyCode;
            Message = message;
        }

        /// <summary>
        /// Gets the Reply Code.
        /// </summary>
        public SmtpReplyCode ReplyCode { get; }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public string Message { get; }
    }

    public enum SmtpReplyCode
    {
        /// <summary>
        /// The server is unable to connect.
        /// </summary>
        UnableToConnect = 101,

        /// <summary>
        /// Connection refused or inability to open an SMTP stream.
        /// </summary>
        ConnectionRefused = 111,

        /// <summary>
        /// System status message or help reply.
        /// </summary>
        SystemMessage = 211,

        /// <summary>
        /// A response to the HELP command.
        /// </summary>
        HelpResponse = 214,

        /// <summary>
        /// The service is ready.
        /// </summary>
        ServiceReady = 220,

        /// <summary>
        /// Goodbye.
        /// </summary>
        ServiceClosingTransmissionChannel = 221,

        /// <summary>
        /// Authentication was Successful.
        /// </summary>
        AuthenticationSuccessful = 235,

        /// <summary>
        /// Everything was Ok.
        /// </summary>
        Ok = 250,

        /// <summary>
        /// "User not local will forward": the recipient’s account is not on the present server, so it will be relayed to another.
        /// </summary>
        RelayToAnotherServer = 251,

        /// <summary>
        /// The server cannot verify the user, but it will try to deliver the message anyway.
        /// </summary>
        CantVerifyUser = 252,

        /// <summary>
        /// Continue with the authentication.
        /// </summary>
        ContinueWithAuth = 334,

        /// <summary>
        /// Start the mail input.
        /// </summary>
        StartMailInput = 354,

        /// <summary>
        /// "Timeout connection problem": there have been issues during the message transfer.
        /// </summary>
        TimeoutConnectionProblem = 420,

        /// <summary>
        /// The service is unavailable due to a connection problem: it may refer to an exceeded limit of simultaneous connections, or a more general temporary problem.
        /// The server (yours or the recipient's) is not available at the moment, so the dispatch will be tried again later.
        /// </summary>
        ServiceUnavailable = 421,

        /// <summary>
        /// The recipient’s mailbox has exceeded its storage limit.
        /// </summary>
        ExceededStorage = 422,

        /// <summary>
        /// Not enough space on the disk, or an "out of memory" condition due to a file overload.
        /// </summary>
        Overloaded = 431,

        /// <summary>
        /// The recipient’s server is not responding.
        /// </summary>
        RecipientNotResponding = 441,

        /// <summary>
        /// The connection was dropped during the transmission.
        /// </summary>
        ConnectionDropped = 442,

        /// <summary>
        /// The maximum hop count was exceeded for the message: an internal loop has occurred.
        /// </summary>
        MaxHopCountExceeded = 446,

        /// <summary>
        /// Your outgoing message timed out because of issues concerning the incoming server.
        /// </summary>
        MessageTimeout = 447,

        /// <summary>
        /// A routing error.
        /// </summary>
        RoutingError = 449,

        /// <summary>
        /// "Requested action not taken – The user’s mailbox is unavailable". The mailbox has been corrupted or placed on an offline server, or your email hasn't been accepted for IP problems or blacklisting.
        /// </summary>
        Unavailable = 450,

        /// <summary>
        /// "Requested action aborted – Local error in processing". Your ISP's server or the server that got a first relay from yours has encountered a connection problem.
        /// </summary>
        Aborted = 451,

        /// <summary>
        /// There is insufficent stored to handle the mail.
        /// </summary>
        InsufficientStorage = 452,

        /// <summary>
        /// The client is not permitted to connect.
        /// </summary>
        ClientNotPermitted = 454,

        /// <summary>
        /// An error of your mail server, often due to an issue of the local anti-spam filter.
        /// </summary>
        Error = 471,

        /// <summary>
        /// Syntax error, command unrecognized (This may include errors such as command line too long).
        /// </summary>
        CommandUnrecognized = 500,

        /// <summary>
        /// Syntax error in parameters or arguments.
        /// </summary>
        SyntaxError = 501,

        /// <summary>
        /// The command has not been implemented.
        /// </summary>
        CommandNotImplemented = 502,

        /// <summary>
        /// Bad sequence of commands.
        /// </summary>
        BadSequence = 503,

        /// <summary>
        /// A command parameter is not implemented.
        /// </summary>
        CommandParameterNotImplemented = 504,

        /// <summary>
        /// Bad email address.
        /// Codes 510 or 511 result the same structure.
        /// One of the addresses in your TO, CC or BBC line doesn't exist. Check again your recipients' accounts and correct any possible misspelling.
        /// </summary>
        BadEmailAddress = 510,

        /// <summary>
        /// A DNS error: the host server for the recipient's domain name cannot be found.
        /// </summary>
        DnsError = 512,

        /// <summary>
        /// "Address type is incorrect": another problem concerning address misspelling. In few cases, however, it's related to an authentication issue.
        /// </summary>
        IncorrectAddressType = 513,

        /// <summary>
        /// The total size of your mailing exceeds the recipient server's limits.
        /// </summary>
        MailingLimitExceeded = 523,

        /// <summary>
        /// Authentication required
        /// </summary>
        AuthenticationRequired = 530,

        /// <summary>
        /// Authentication failed.
        /// </summary>
        AuthenticationFailed = 535,

        /// <summary>
        /// The recipient address rejected your message: normally, it's an error caused by an anti-spam filter.
        /// </summary>
        RecipientAddressRejected = 541,

        /// <summary>
        /// The Mailbox is temporarily unavailable.
        /// </summary>
        MailboxUnavailable = 550,

        /// <summary>
        /// "User not local or invalid address – Relay denied". Meaning, if both your address and the recipients are not locally hosted by the server, a relay can be interrupted.
        /// </summary>
        RelayDenied = 551,

        /// <summary>
        /// The size limit has been exceeded.
        /// </summary>
        SizeLimitExceeded = 552,

        /// <summary>
        /// The Mailbox is permanently not available.
        /// </summary>
        MailboxNameNotAllowed = 553,

        /// <summary>
        /// The transaction failed.
        /// </summary>
        TransactionFailed = 554
    }

    internal sealed class SmtpStateTable : IEnumerable
    {
        internal static readonly SmtpStateTable Shared = new SmtpStateTable
        {
            new SmtpState(SmtpStateId.Initialized)
            {
                { NoopCommand.Command },
                { RsetCommand.Command },
                { QuitCommand.Command },
                { ProxyCommand.Command },
                { HeloCommand.Command, WaitingForMailSecureWhenSecure },
                { EhloCommand.Command, WaitingForMailSecureWhenSecure }
            },
            new SmtpState(SmtpStateId.WaitingForMail)
            {
                { NoopCommand.Command },
                { RsetCommand.Command },
                { QuitCommand.Command },
                { StartTlsCommand.Command, CanAcceptStartTls, SmtpStateId.WaitingForMailSecure },
                { AuthCommand.Command, context => context.EndpointDefinition.AllowUnsecureAuthentication && context.Authentication.IsAuthenticated == false },
                { HeloCommand.Command, SmtpStateId.WaitingForMail },
                { EhloCommand.Command, SmtpStateId.WaitingForMail },
                { MailCommand.Command, SmtpStateId.WithinTransaction }
            },
            new SmtpState(SmtpStateId.WaitingForMailSecure)
            {
                { NoopCommand.Command },
                { RsetCommand.Command },
                { QuitCommand.Command },
                { AuthCommand.Command, context => context.Authentication.IsAuthenticated == false },
                { HeloCommand.Command, SmtpStateId.WaitingForMailSecure },
                { EhloCommand.Command, SmtpStateId.WaitingForMailSecure },
                { MailCommand.Command, SmtpStateId.WithinTransaction }
            },
            new SmtpState(SmtpStateId.WithinTransaction)
            {
                { NoopCommand.Command },
                { RsetCommand.Command, WaitingForMailSecureWhenSecure },
                { QuitCommand.Command },
                { RcptCommand.Command, SmtpStateId.CanAcceptData },
            },
            new SmtpState(SmtpStateId.CanAcceptData)
            {
                { NoopCommand.Command },
                { RsetCommand.Command, WaitingForMailSecureWhenSecure },
                { QuitCommand.Command },
                { RcptCommand.Command },
                { DataCommand.Command, SmtpStateId.WaitingForMail },
            }
        };

        static SmtpStateId WaitingForMailSecureWhenSecure(SmtpSessionContext context)
        {
            return context.Pipe.IsSecure ? SmtpStateId.WaitingForMailSecure : SmtpStateId.WaitingForMail;
        }

        static bool CanAcceptStartTls(SmtpSessionContext context)
        {
            return context.EndpointDefinition.ServerCertificate != null && context.Pipe.IsSecure == false;
        }

        readonly IDictionary<SmtpStateId, SmtpState> _states = new Dictionary<SmtpStateId, SmtpState>();

        internal SmtpState this[SmtpStateId stateId] => _states[stateId];

        /// <summary>
        /// Add the state to the table.
        /// </summary>
        /// <param name="state"></param>
        void Add(SmtpState state)
        {
            _states.Add(state.StateId, state);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            // this is just here for the collection initializer syntax to work
            throw new NotImplementedException();
        }
    }


    internal enum SmtpStateId
    {
        None = 0,
        Initialized = 1,
        WaitingForMail = 2,
        WaitingForMailSecure = 3,
        WithinTransaction = 4,
        CanAcceptData = 5,
    }

    internal sealed class SmtpState : IEnumerable
    {
        internal SmtpState(SmtpStateId stateId)
        {
            StateId = stateId;
        }

        internal void Add(string command)
        {
            Transitions.Add(command, new SmtpStateTransition(context => true, context => StateId));
        }

        internal void Add(string command, SmtpStateId state)
        {
            Transitions.Add(command, new SmtpStateTransition(context => true, context => state));
        }

        internal void Add(string command, Func<SmtpSessionContext, SmtpStateId> transitionDelegate)
        {
            Transitions.Add(command, new SmtpStateTransition(context => true, transitionDelegate));
        }

        internal void Add(string command, Func<SmtpSessionContext, bool> canAcceptDelegate)
        {
            Transitions.Add(command, new SmtpStateTransition(canAcceptDelegate, context => StateId));
        }

        internal void Add(string command, Func<SmtpSessionContext, bool> canAcceptDelegate, SmtpStateId state)
        {
            Transitions.Add(command, new SmtpStateTransition(canAcceptDelegate, context => state));
        }

        internal void Add(string command, Func<SmtpSessionContext, bool> canAcceptDelegate, Func<SmtpSessionContext, SmtpStateId> transitionDelegate)
        {
            Transitions.Add(command, new SmtpStateTransition(canAcceptDelegate, transitionDelegate));
        }

        // this is just here for the collection initializer syntax to work
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        internal SmtpStateId StateId { get; }

        internal IDictionary<string, SmtpStateTransition> Transitions { get; } = new Dictionary<string, SmtpStateTransition>(StringComparer.OrdinalIgnoreCase);
    }


    internal sealed class SmtpStateTransition
    {
        readonly Func<SmtpSessionContext, bool> _canAcceptDelegate;
        readonly Func<SmtpSessionContext, SmtpStateId> _transitionDelegate;

        internal SmtpStateTransition(Func<SmtpSessionContext, bool> canAcceptDelegate, Func<SmtpSessionContext, SmtpStateId> transitionDelegate)
        {
            _canAcceptDelegate = canAcceptDelegate;
            _transitionDelegate = transitionDelegate;
        }

        internal bool CanAccept(SmtpSessionContext context)
        {
            return _canAcceptDelegate(context);
        }

        internal SmtpStateId Transition(SmtpSessionContext context)
        {
            return _transitionDelegate(context);
        }
    }
    public sealed class NoopCommand : SmtpCommand
    {
        public const string Command = "NOOP";

        /// <summary>
        /// Constructor.
        /// </summary>
        public NoopCommand() : base(Command) { }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="context">The execution context to operate on.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns true if the command executed successfully such that the transition to the next state should occurr, false 
        /// if the current state is to be maintained.</returns>
        internal override async Task<bool> ExecuteAsync(SmtpSessionContext context, CancellationToken cancellationToken)
        {
            await context.Pipe.Output.WriteReplyAsync(SmtpResponse.Ok, cancellationToken).ConfigureAwait(false);

            return true;
        }
    }

    public sealed class SmtpResponseException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="response">The response to raise in the exception.</param>
        public SmtpResponseException(SmtpResponse response) : this(response, false) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="response">The response to raise in the exception.</param>
        /// <param name="quit">Indicates whether or not the session should terminate.</param>
        public SmtpResponseException(SmtpResponse response, bool quit)
        {
            Response = response;
            IsQuitRequested = quit;
        }

        /// <summary>
        /// The response to return to the client.
        /// </summary>
        public SmtpResponse Response { get; }

        /// <summary>
        /// Indicates whether or not the session should terminate.
        /// </summary>
        public bool IsQuitRequested { get; }
    }
}
