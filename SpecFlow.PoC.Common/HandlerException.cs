namespace SpecFlow.PoC.Common
{
    public class HandlerException : Exception
    {
        public HandlerException(string handlerName, string mainMessage, IEnumerable<ErrorMessage> messages = null)
            : base(mainMessage)
        {
            HandlerName = handlerName;
            Messages = messages != null ? messages.ToList() : new List<ErrorMessage>();
        }

        public HandlerException(string handlerName, string mainMessage, Exception innerException, IEnumerable<ErrorMessage> messages = null)
            : base(mainMessage, innerException)
        {
            HandlerName = handlerName;
            Messages = messages != null ? messages.ToList() : new List<ErrorMessage>();
        }

        public HandlerException(string message)
            : base(message)
        {
            Messages = new List<ErrorMessage>();
        }

        public HandlerException(string message, Exception innerException)
            : base(message, innerException)
        {
            Messages = new List<ErrorMessage>();
        }

        protected HandlerException()
        {
            Messages = new List<ErrorMessage>();
        }

        public string HandlerName { get; set; }

        public List<ErrorMessage> Messages { get; private set; }
    }
}
