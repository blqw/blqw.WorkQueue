using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blqw
{
    sealed class Worker
    {
        internal Worker(Guid workid)
        {
            ID = workid;
        }

        public Worker(IWork work)
        {
            if (work == null)
            {
                new ArgumentNullException("work");
            }
            _Work = work;
            ID = Guid.NewGuid();
        }

        public Guid ID { get; private set; }
        public Exception Error { get; set; }
        private int _Status;
        private IWork _Work;
        public WorkStatus Status
        {
            get
            {
                return (WorkStatus)_Status;
            }
        }

        public bool SetStatus(WorkStatus old, WorkStatus @new)
        {
            var raw = Interlocked.CompareExchange(ref _Status, (int)@new, (int)old);
            return (raw == (int)old);
        }

        public async Task Execute()
        {
            if (SetStatus(WorkStatus.Created, WorkStatus.Running))
            {
                try
                {
                    Trace.Close();
                    Trace.WriteLine("任务id:" + ID.ToString("n"), "WorkQueue");
                    System.Threading.SynchronizationContext.SetSynchronizationContext(null);
                    await _Work.Execute().ConfigureAwait(false);
                    _Status = (int)WorkStatus.Completion;
                }
                catch (Exception ex)
                {
                    Error = ex;
                    _Status = (int)WorkStatus.Faulted;
                }
                finally
                {
                    Trace.Flush();
                }
            }
        }
    }
}
