using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventLostCenter.Tactics
{
    /// <summary>
    /// 上盖打开
    /// </summary>
    public class CoverOpenASB:PreventLostSuper
    {
        public override PrinterState Handling()
        {
            Debug.WriteLine("打印机上盖打开");
            return PrinterState.CoverOpen;
        }
    }
}
