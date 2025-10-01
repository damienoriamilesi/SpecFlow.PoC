using System.Diagnostics;

namespace SpecFlow.PoC.Common
{
    [DebuggerStepThrough]
    public abstract class HandlerBase<TParameters, TResults>
    {
        protected HandlerBase<TParameters, TResults> Successor { get; private set; }

        #region Process events

        public event EventHandler<AlertingEventArgs> Alerting;

        public event EventHandler<EventArgs> ExecuteStarted;

        public event EventHandler<ExecuteTerminatedEventArgs<TParameters, TResults>> ExecuteTerminated;

        public event EventHandler<FailedEventArgs<TResults>> Failed;

        public event EventHandler<EventArgs> ProcessStarted;

        public event EventHandler<EventArgs> ProcessSuccessor;

        public event EventHandler<EventArgs> ProcessTerminated;

        private void OnAlerting(AlertingEventArgs e)
        {
            if (Alerting == null)
                return;
            Alerting(this, e);
        }

        private void OnExecuteStarted(EventArgs e)
        {
            if (ExecuteStarted == null)
                return;
            ExecuteStarted(this, e);
        }

        private void OnExecuteTerminated(ExecuteTerminatedEventArgs<TParameters, TResults> e)
        {
            if (ExecuteTerminated == null)
                return;
            ExecuteTerminated(this, e);
        }

        private void OnFailed(FailedEventArgs<TResults> e)
        {
            if (Failed == null)
                return;
            Failed(this, e);
        }

        private void OnProcessStarted(EventArgs e)
        {
            if (ProcessStarted == null)
                return;
            ProcessStarted(this, e);
        }

        private void OnProcessSuccessor(EventArgs e)
        {
            if (ProcessSuccessor == null)
                return;
            ProcessSuccessor(this, e);
        }

        private void OnProcessTerminated(EventArgs e)
        {
            if (ProcessTerminated == null)
                return;
            ProcessTerminated(this, e);
        }

        #endregion Process events

        public void ProcessRequest(HandleRequest<TParameters, TResults> request)
        {
            OnProcessStarted(new EventArgs());
            if (CanProcess(request))
            {
                // Start handler timer
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                DateTime startedAt = DateTime.Now;

                OnExecuteStarted(new EventArgs());
                try
                {
                    ProcessHandler(request);
                }
                catch (Exception ex)
                {
                    OnFailed(new FailedEventArgs<TResults>(request.Results, ex));
                    throw;
                }
                // Stop handler timer
                stopwatch.Stop();

                OnExecuteTerminated(new ExecuteTerminatedEventArgs<TParameters, TResults>(request, startedAt, stopwatch.Elapsed));
            }
            if (Successor != null)
            {
                OnProcessSuccessor(new EventArgs());
                Successor.ProcessRequest(request);
            }
            OnProcessTerminated(new EventArgs());
        }

        public void SetSuccessor(HandlerBase<TParameters, TResults> successor)
        {
            if (successor == null)
                throw new ArgumentNullException("successor");
            Successor = successor;
        }

        protected void Alert(string message)
        {
            OnAlerting(new AlertingEventArgs(message));
        }

        protected virtual bool CanProcess(HandleRequest<TParameters, TResults> request)
        {
            return true;
        }

        protected abstract void Execute(HandleRequest<TParameters, TResults> request);

        protected void ProcessHandler(HandleRequest<TParameters, TResults> request)
        {
            try
            {
                Execute(request);
            }
            catch (HandlerException)
            {
                // Si une TException est levée, on sort directement.
                throw;
            }
            catch (Exception ex)
            {
                // Toutes les exceptions împrévues sont attrapées ici et permettent au Handler de sortir proprement.
                // Si un message plus précis doit être fourni, lever une TException avec le bon message d'erreur.
                ThrowException(
                    String.Format("Une erreur imprévue est survenue pendant l'étape '{0}'.", GetType().FullName), null, ex);
            }
        }

        protected void ThrowDisplayedException(string message, IEnumerable<ErrorMessage> messages, Exception innerException = null)
        {
            throw new DisplayedHandlerException(GetType().Name, message, innerException, messages);
        }

        protected void ThrowDisplayedException(string message, Exception innerException = null)
        {
            ThrowDisplayedException(message, null, innerException);
        }

        protected void ThrowException(string mainMessage, IEnumerable<ErrorMessage> messages, Exception innerException = null)
        {
            throw new HandlerException(GetType().Name, mainMessage, innerException, messages);
        }

        protected void ThrowException(string mainMessage, Exception innerException)
        {
            ThrowException(mainMessage, null, innerException);
        }

        protected void ThrowException(string mainMessage)
        {
            ThrowException(mainMessage, null);
        }
    }
}
