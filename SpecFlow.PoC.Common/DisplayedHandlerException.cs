namespace SpecFlow.PoC.Common
{
    public class DisplayedHandlerException : HandlerException
    {
        public DisplayedHandlerException(string handlerName, string mainMessage, IEnumerable<ErrorMessage> messages = null)
            : base(handlerName, mainMessage, messages)
        { }

        public DisplayedHandlerException(string handlerName, string mainMessage, Exception innerException, IEnumerable<ErrorMessage> messages = null)
            : base(handlerName, mainMessage, innerException, messages)
        {
        }

        public DisplayedHandlerException(string message)
            : base(message)
        {
        }

        public DisplayedHandlerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DisplayedHandlerException()
        {
        }
    }
}
