using System.Diagnostics;
using System.Transactions;

namespace SpecFlow.PoC.Common
{
    [DebuggerStepThrough]
    public abstract class ProcessBase<TParameters, TResults>

        where TResults : IResults, new()
    {
        private HandlerBase<TParameters, TResults>[] _handlers;

        public virtual TimeSpan DurationLimit { get { return TimeSpan.FromSeconds(5); } }
        public virtual TimeSpan TransactionScopeTimeout { get { return TransactionManager.DefaultTimeout; } }

        public virtual TResults Execute(TParameters parameters, string exceptionLessApiKey = null, string exceptionLessServerUrl = null)
        {
            // Start timer
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Defines handlers
            _handlers = GetHandlers();

            // Define process handlers
            _handlers = AppendProcessHandlers(_handlers);

            // Enable logging if necessary
            if (this is IEnableLogging)
            {
                if (string.IsNullOrEmpty(exceptionLessApiKey) && string.IsNullOrEmpty(exceptionLessServerUrl))
                {
                    //ExceptionLogClient.Instance.Startup(ConfigurationManager.AppSettings["ExceptionLessApiKey"], ConfigurationManager.AppSettings["ExceptionLessServerUrl"]);
                }
                else
                {
                    //ExceptionLogClient.Instance.Startup(exceptionLessApiKey, exceptionLessServerUrl);
                }
            }

            // Chain handlers
            for (int i = 0; i < _handlers.Length; i++)
            {
                if (i < _handlers.Length - 1)
                    _handlers[i].SetSuccessor(_handlers[i + 1]);
                _handlers[i].Failed += HandlerOnFailed;
                _handlers[i].ProcessStarted += HandlerProcessStarted;
                _handlers[i].ProcessTerminated += HandlerProcessTerminated;
                _handlers[i].ExecuteStarted += HandlerExecuteStarted;
                _handlers[i].ExecuteTerminated += HandlerExecuteTerminated;
                _handlers[i].ProcessSuccessor += HandlerProcessSuccessor;
                _handlers[i].Alerting += HandlerAlerting;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} : ProcessHandler started", GetType().Name);
            HandleRequest<TParameters, TResults> request = new HandleRequest<TParameters, TResults> { Parameters = parameters, Results = new TResults() };

            request.ExecutionResult.StartedAt = DateTime.Now;

            try
            {
                PreExecute(request);
                // ProcessHandler the steps
                if (this is IRequireTransactionScope)
                {
                    using (var transcactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { Timeout = TransactionScopeTimeout, IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        _handlers[0].ProcessRequest(request);
                        transcactionScope.Complete();
                    }
                }
                else
                    _handlers[0].ProcessRequest(request);
            }
            catch (Exception ex)
            {
                request.Results.Exception = ex;
            }
            finally
            {
                PostExecute(request);
            }

            // Stop timer
            stopwatch.Stop();
            request.ExecutionResult.Duration = stopwatch.Elapsed;

            // Log the feature
            if (this is IEnableFeatureLogging) { }
                /*ExceptionLogClient.Instance.CreateFeatureLog(GetType().Name)
                    .AddTags(GetType().Name)
                    .AddObject("parameters", request.Parameters)
                    .AddObject("results", request.Results)
                    .AddObject("executionresult", request.ExecutionResult)
                    .Submit();*/

            // Log the exception
            if (this is IEnableExceptionLogging
                && request.Results.Exception != null)
            {
                /*request.Results.Exception.ToExceptionLog()
                    .AddTags(GetType().Name)
                    .AddObject("parameters", request.Parameters)
                    .AddObject("results", request.Results)
                    .AddObject("executionresult", request.ExecutionResult)
                    .Submit();*/
            }

            // Log the exceeded duration
            if (this is IEnableExceededDurationLogging
                && request.ExecutionResult.Duration > DurationLimit){ }
                /*ExceptionLogClient.Instance.CreateWarningLog(GetType().Name,
                    string.Format("Le process a duré plus de {0} s ({1} s).", DurationLimit.TotalSeconds,
                        request.ExecutionResult.Duration.TotalSeconds))
                    .AddObject("parameters", request.Parameters)
                    .AddObject("results", request.Results)
                    .AddObject("executionresult", request.ExecutionResult)
                    .Submit();*/

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} : ProcessHandler terminated ({1})", GetType().Name, stopwatch.Elapsed);
            return request.Results;
        }

        protected virtual HandlerBase<TParameters, TResults>[] AppendProcessHandlers(HandlerBase<TParameters, TResults>[] handlers)
        {
            return handlers;
        }

        protected abstract HandlerBase<TParameters, TResults>[] GetHandlers();

        protected virtual void PostExecute(HandleRequest<TParameters, TResults> request)
        {
        }

        protected virtual void PreExecute(HandleRequest<TParameters, TResults> request)
        {
        }

        private void HandlerAlerting(object sender, AlertingEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} : {1} : {2}", DateTime.Now, sender.GetType().Name, e.Message);
        }

        private void HandlerExecuteStarted(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} : {1} : ProcessHandler started", DateTime.Now, sender.GetType().Name);
        }

        private void HandlerExecuteTerminated(object sender, ExecuteTerminatedEventArgs<TParameters, TResults> e)
        {
            e.Request.ExecutionResult.Items.Add(new ExecutionResult
            {
                Name = sender.GetType().Name,
                StartedAt = e.StartedAt,
                Duration = e.Duration
            });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} : {1} : ProcessHandler terminated", DateTime.Now, sender.GetType().Name);
        }

        private void HandlerOnFailed(object sender, FailedEventArgs<TResults> e)
        {
            e.Results.Exception = e.Exception;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} : {1} : {2}", DateTime.Now, sender.GetType().Name, e.Exception.GetBaseException().Message);
        }

        private void HandlerProcessStarted(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("{0} : {1} : Process started", DateTime.Now, sender.GetType().Name);
        }

        private void HandlerProcessSuccessor(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("{0} : {1} : Process Successor", DateTime.Now, sender.GetType().Name);
        }

        private void HandlerProcessTerminated(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("{0} : {1} : Process terminated", DateTime.Now, sender.GetType().Name);
        }
    }
}
