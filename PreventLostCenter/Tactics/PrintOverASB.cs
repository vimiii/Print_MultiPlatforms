using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventLostCenter.Tactics
{
    /// <summary>
    /// 一次打印结束
    /// </summary>
    class PrintOverASB:PreventLostSuper
    {
        public override PrinterState Handling(string IP)
        {
            Debug.WriteLine(IP+"打印机正常");
            return PrinterState.PrintOver;
        }
    }
}
