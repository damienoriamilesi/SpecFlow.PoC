namespace SpecFlow.PoC.Common
{
    public class EnableLoggingHandler<TParameters, TResults> : HandlerBase<TParameters, TResults>
    {
        protected override void Execute(HandleRequest<TParameters, TResults> request)
        {
           // ExceptionLogClient.Instance.Startup(ConfigurationManager.AppSettings["ExceptionLessApiKey"],
             //   ConfigurationManager.AppSettings["ExceptionLessServerUrl"]);
        }
    }
}
