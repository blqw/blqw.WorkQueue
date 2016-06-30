using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 工作队列
    /// </summary>
    public class WorkQueue
    {
        LinkedList<Worker> _Workers = new LinkedList<Worker>();
        UsingLock2 _Lock = new UsingLock2();
        int _Count = 0;
        /// <summary> 当期工作线程
        /// </summary>
        int _WorkingCount = 0;

        /// <summary> 初始化工作队列
        /// </summary>
        /// <param name="maxParallelWorksCount">最大并行工作的任务数</param>
        public WorkQueue(int maxParallelWorksCount = 1)
        {
            if (maxParallelWorksCount < 1 || maxParallelWorksCount > 500)
            {
                throw new ArgumentOutOfRangeException("maxParallelWorksCount", "maxParallelWorksCount不能小于1或者大于500");
            }
            MaxParallelWorksCount = maxParallelWorksCount;
            IsRuning = true;
        }

        /// <summary> 将任务添加到队列的尾端,并按添加的先后顺序依次执行
        /// </summary>
        /// <param name="work">任务实体</param>
        /// <returns></returns>
        public Guid Add(IWork work)
        {
            var w = new Worker(work);
            using (_Lock.Write())
            {
                if (Count > 100000)
                {
                    throw new ArgumentOutOfRangeException("Count", "队列中排队的任务不能超过100000");
                }
                _Workers.AddLast(w);
                Interlocked.Increment(ref _Count);
            }
            if (IsRuning) StartOne();
            return w.ID;
        }

        /// <summary> 将任务插入到队列最开始的位置,并在接下来立即执行,返回任务id
        /// </summary>
        /// <param name="work">任务实体</param>
        /// <returns></returns>
        public Guid Insert(IWork work)
        {
            var w = new Worker(work);
            using (_Lock.Write())
            {
                if (Count > 100000)
                {
                    throw new ArgumentOutOfRangeException("Count", "队列中排队的任务不能超过100000");
                }
                _Workers.AddFirst(w);
                Interlocked.Increment(ref _Count);
            }
            if (IsRuning) StartOne();
            return w.ID;
        }

        /// <summary> 取消指定的任务(任务尚未执行才有效)
        /// </summary>
        /// <param name="workid">任务id</param>
        /// <returns></returns>
        public bool Cencel(Guid workid)
        {
            using (_Lock.Read())
            {
                var node = _Workers.Find(new Worker(workid));
                if (node != null)
                {
                    Interlocked.Decrement(ref _Count);
                    return node.Value.SetStatus(WorkStatus.Created, WorkStatus.Canceled);
                }
            }
            return false;
        }

        /// <summary> 清除队列中所有未执行的任务
        /// </summary>
        public void Clear()
        {
            using (_Lock.Write())
            {
                _Workers.Clear();
            }
        }


        /// <summary> 立即从当前队列中获取未执行的任务并执行
        /// </summary>
        public void Start()
        {
            IsRuning = true;
            int count = 0;
            lock (_Lock)
            {
                if (_WorkingCount >= MaxParallelWorksCount)
                {
                    return;
                }
                count =  MaxParallelWorksCount - _WorkingCount;
                _WorkingCount = MaxParallelWorksCount;
            }
            //Parallel.For(0, count, StartOneAsync);
            for (int i = 0; i < count; i++)
            {
                StartOneAsync();
            }
        }

        /// <summary> 停止从当前队列中获取未执行的任务并执行的操作(正在执行的任务不受影响)
        /// </summary>
        public void Stop()
        {
            IsRuning = false;
        }

        /// <summary> 最大并行工作的任务数
        /// </summary>
        public int MaxParallelWorksCount { get; private set; }

        /// <summary> 当前队列中任务总数
        /// </summary>
        public int Count { get { return _Count; } }

        /// <summary> 是否正在运行
        /// </summary>
        public bool IsRuning { get; private set; }

        private void StartOne()
        {
            lock (_Lock)
            {
                if (_WorkingCount >= MaxParallelWorksCount)
                {
                    return;
                }
                _WorkingCount++;
            }
            StartOneAsync();
        }

        /// <summary> 
        /// </summary>
        private async void StartOneAsync()
        {
            await Task.Delay(1);
#if DEBUG
            Interlocked.Increment(ref _ActualWorkingCount);
#endif
            //将状态改为 停止
            while (true)
            {
                Worker worker;
                using (_Lock.Write())
                {
                    if (IsRuning == false || Count == 0)
                    {
                        lock (_Lock)
                        {
                            _WorkingCount--;
                            break;
                        }
                    }
                    worker = _Workers.First.Value;
                    _Workers.RemoveFirst();
                    if (worker.Status == WorkStatus.Canceled)
                    {
                        continue;
                    }
                    Interlocked.Decrement(ref _Count);
                }
                await worker.Execute();
            }
#if DEBUG
            Interlocked.Decrement(ref _ActualWorkingCount);
#endif
        }


#if DEBUG
        /// <summary> 实际工作线程数
        /// </summary>
        int _ActualWorkingCount;
#endif

        /// <summary> 返回当前队列中的总任务数和当前执行线程数
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
#if DEBUG
            return string.Concat(
                "当前任务数:", Count.ToString(),
                " 申请执行:", _WorkingCount.ToString(),
                " 实际执行中:", _ActualWorkingCount.ToString());
#else
            return string.Concat(
                "当前任务数:", Count.ToString(),
                " 执行中:", Math.Min(_WorkingCount, MaxParallelWorksCount).ToString());
#endif
        }
    }
}
