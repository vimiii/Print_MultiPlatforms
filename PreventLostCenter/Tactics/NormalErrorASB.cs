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
    public class NormalErrorASB : PreventLostSuper
    {
        public override PrinterState Handling()
        {
            Debug.WriteLine("打印机有可恢复异常");
            return PrinterState.NormalError;
        }
    }
}
