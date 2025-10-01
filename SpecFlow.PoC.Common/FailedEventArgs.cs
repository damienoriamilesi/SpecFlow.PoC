namespace SpecFlow.PoC.Common
{
    public class FailedEventArgs<TResults> : EventArgs
    {
        public FailedEventArgs(TResults results, Exception exception)
        {
            Results = results;
            Exception = exception;
        }

        public TResults Results { get; set; }

        public Exception Exception { get; set; }
    }
}
