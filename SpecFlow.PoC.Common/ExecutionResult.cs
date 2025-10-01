namespace SpecFlow.PoC.Common
{
    public class ExecutionResult
    {
        public ExecutionResult()
        {
            Items = new List<ExecutionResult>();
        }

        public TimeSpan Duration { get; set; }
        public List<ExecutionResult> Items { get; private set; }
        public string Name { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
