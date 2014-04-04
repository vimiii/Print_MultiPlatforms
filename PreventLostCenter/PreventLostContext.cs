using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventLostCenter
{
    public class PreventLostContext
    {
        
        PreventLostSuper super;
        public PreventLostContext(byte[] asb,PrintStateMachine state)
        {
            super = PreventLostFactary.HandlerFactory(asb,state);
        }
        public void Tactic()
        { 
            if(super!=null)
            {
                super.Handling();
            }
        }
    }
}
