namespace SpecFlow.PoC.Common
{
    public class AlertingEventArgs : EventArgs
    {
        public AlertingEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
