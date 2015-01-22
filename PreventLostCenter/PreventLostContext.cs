using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventLostCenter
{
    public class PreventLostContext
    {
        
        PreventLostSuper super;
        public PreventLostContext(byte[] asb)
        {
            super = PreventLostFactary.HandlerFactory(asb);
        }
        public PrinterState Tactic(string IP)
        { 
            if(super!=null)
            {
                return super.Handling(IP);
            }
            Debug.WriteLine("没有处理该打印机状态");
            return PrinterState.UnCatchPrinter;
        }
    }
}
