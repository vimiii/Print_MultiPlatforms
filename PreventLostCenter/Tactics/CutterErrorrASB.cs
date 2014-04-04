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
    class CutterErrorrASB : PreventLostSuper
    {
        public override void Handling()
        {
            Debug.WriteLine("切刀错误");
        }
    }
}
