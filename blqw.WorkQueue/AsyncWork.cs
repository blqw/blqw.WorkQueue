using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 异步任务
    /// </summary>
    public abstract class AsyncWork : IWork
    {
        /// <summary> 准备中
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnReady()
        {
            return null;
        }
        /// <summary> 执行任务
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnExecuting();
        /// <summary> 执行完毕(出现异常不进该方法)
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnExecuted()
        {
            return null;
        }

        /// <summary> 发生错误时执行
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnError(Exception error)
        {
            return null;
        }

        async Task IWork.Execute()
        {
            Task task;
            Exception error;
            try
            {
                task = OnReady();
                if (task != null) await task;
                task = OnExecuting();
                if (task != null) await task;
                task = OnExecuted();
                if (task != null) await task;
                return;
            }
            catch (Exception ex)
            {
                error = ex;
            }

            try
            {
                task = OnError(error);
                if (task != null) await task;
            }
            catch (Exception ex)
            {
                throw new AggregateException("执行任务发生多个错误", error, ex);
            }

            throw new TargetInvocationException(error);
        }
    }
}
