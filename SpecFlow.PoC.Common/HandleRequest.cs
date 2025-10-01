namespace SpecFlow.PoC.Common
{
    public class HandleRequest<TParameters, TResult>
    {
        public HandleRequest()
        {
            ExecutionResult = new ExecutionResult();
        }

        public ExecutionResult ExecutionResult { get; set; }

        public TParameters Parameters { get; set; }

        public TResult Results { get; set; }
    }
}
