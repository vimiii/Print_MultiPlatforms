using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventLostCenter.Tactics
{
    /// <summary>
    /// 打印机一切正常
    /// </summary>
    public class PrinterOffLineASB : PreventLostSuper
    {
        public override PrinterState Handling(string IP)
        {
            Debug.WriteLine(IP+"打印机离线");
            return PrinterState.PrinterOffLine;
        }
    }
}
