namespace DesignPattern
{
    internal class LowPriority : NotificationPriority
    {
        public LowPriority(INotification notification) : base(notification)
        {
        }

        public override void SendNotification(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}