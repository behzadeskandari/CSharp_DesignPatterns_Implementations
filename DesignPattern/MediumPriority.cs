namespace DesignPattern
{
    public class MediumPriority : NotificationPriority
    {
        public MediumPriority(INotification notification) : base(notification)
        {
        }

        public override void SendNotification(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}