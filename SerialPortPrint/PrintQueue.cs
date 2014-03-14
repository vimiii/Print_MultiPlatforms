using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintBase
{
    /// <summary>
    /// 打印机队列管理
    /// </summary>
    public class PrintQueue
    {
        /// <summary>
        /// 打印队列
        /// </summary>
        public Queue<byte[]> QueueList = new Queue<byte[]>();   
    }
}
