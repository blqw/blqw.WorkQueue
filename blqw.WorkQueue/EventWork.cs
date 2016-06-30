using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 使用事件方式触发的任务实体
    /// </summary>
    public sealed class EventWork : Work
    {
        /// <summary> 准备中
        /// </summary>
        /// <returns></returns>
        public event EventHandler Ready;
        /// <summary> 执行任务
        /// </summary>
        public event EventHandler Executing;
        /// <summary> 执行完毕(出现异常触发该事件)
        /// </summary>
        public event EventHandler Executed;

        /// <summary> 发生错误时执行
        /// </summary>
        /// <returns></returns>
        public event EventHandler Error;


        /// <summary> 触发Ready事件
        /// </summary>
        protected override void OnReady()
        {
            var temp = Ready;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        /// <summary> 触发Error事件
        /// </summary>
        protected override void OnError(Exception error)
        {
            var temp = Error;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        /// <summary> 触发Executed事件
        /// </summary>
        protected override void OnExecuted()
        {
            var temp = Executed;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        /// <summary> 触发Executing事件
        /// </summary>
        protected override void OnExecuting()
        {
            var temp = Executing;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }
    }
}
