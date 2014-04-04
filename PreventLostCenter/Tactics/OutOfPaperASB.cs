using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventLostCenter.Tactics
{
    /// <summary>
    /// 打印机缺纸
    /// </summary>
    internal class OutOfPaperASB:PreventLostSuper
    {
        public override void Handling()
        {
            Debug.WriteLine("打印机缺纸");
        }
    }
}
