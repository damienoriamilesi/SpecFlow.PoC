namespace SpecFlow.PoC.Common
{
    public class ExecuteTerminatedEventArgs<TParameters, TResults> : EventArgs
    {
        public ExecuteTerminatedEventArgs(HandleRequest<TParameters, TResults> request, DateTime startedAt, TimeSpan duration)
        {
            Request = request;
            StartedAt = startedAt;
            Duration = duration;
        }

        public TimeSpan Duration { get; set; }
        public HandleRequest<TParameters, TResults> Request { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
