using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum WorkStatus
    {
        /// <summary> 该任务已初始化，但尚未被计划。
        /// </summary>
        Created = 0,
        /// <summary> 该任务正在运行，但尚未完成。
        /// </summary>
        Running = 2,
        /// <summary> 已成功完成执行的任务。
        /// </summary>
        Completion = 3,
        /// <summary> 任务被取消
        /// </summary>
        Canceled = 4,
        /// <summary> 由于未处理异常的原因而完成的任务。
        /// </summary>
        Faulted = 5,
    }
}
