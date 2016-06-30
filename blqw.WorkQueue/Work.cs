using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 任务实体
    /// </summary>
    public abstract class Work : IWork
    {
        /// <summary> 准备中
        /// </summary>
        /// <returns></returns>
        protected virtual void OnReady()
        {

        }
        /// <summary> 执行任务
        /// </summary>
        /// <returns></returns>
        protected abstract void OnExecuting();
        /// <summary> 执行完毕(出现异常不进该方法)
        /// </summary>
        /// <returns></returns>
        protected virtual void OnExecuted()
        {

        }

        /// <summary> 发生错误时执行
        /// </summary>
        /// <returns></returns>
        protected virtual void OnError(Exception error)
        {

        }

        Task IWork.Execute()
        {
            try
            {
                OnReady();
                OnExecuting();
                OnExecuted();
            }
            catch (Exception ex)
            {
                try
                {
                    OnError(ex);
                }
                catch (Exception ex2)
                {
                    throw new AggregateException("执行任务发生多个错误", ex, ex2);
                }
                throw;
            }

            return null;
        }
    }
}
