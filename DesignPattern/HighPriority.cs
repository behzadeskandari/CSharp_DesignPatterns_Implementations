namespace DesignPattern
{
    internal class HighPriority : NotificationPriority
    {
        public HighPriority(INotification notification) : base(notification)
        {
        }

        public override void SendNotification(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}