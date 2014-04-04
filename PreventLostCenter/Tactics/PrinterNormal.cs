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
    public class PrinterNormal:PreventLostSuper
    {
        public override void Handling()
        {
            Debug.WriteLine("打印机正常");
        }
    }
}
