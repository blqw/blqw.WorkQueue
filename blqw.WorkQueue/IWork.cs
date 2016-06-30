using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    /// <summary> 工作任务
    /// </summary>
    public interface IWork
    {
        /// <summary> 执行工作
        /// </summary>
        /// <returns></returns>
        Task Execute();
    }
}
