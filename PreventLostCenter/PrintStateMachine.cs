using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PreventLostCenter
{
    /// <summary>
    /// 打印状态机
    /// </summary>
    internal class PrintStateMachine
    {
        /// <summary>
        /// 打印机当前状态(-1：实时清除打印机状态)
        /// </summary>
        public int State;
        public PrintStateMachine()
        {
            State=1;
        }
        public bool IsPrintNormal()
        {
            return State == 1;
        }
        
    }
}
