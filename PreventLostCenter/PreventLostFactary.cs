using PreventLostCenter.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventLostCenter
{
    internal class PreventLostFactary
    {
        public static PreventLostSuper HandlerFactory(byte[] asb)
        {
            PrintStateMachine state = new PrintStateMachine();
            if (asb == null || asb.Length != 4)
            {
                return null;
            }
            byte first = asb[0];
            byte second = asb[1];
            byte third = asb[2];
            byte forth = asb[3];

            if ((first & 32) == (byte)32)
            {
                
                //打印机上盖打开
                return new CoverOpenASB();
            }
            
            if ((second & 8) == (byte)8)
            {
                
                //有切刀错误
                return new CutterErrorrASB();
            }
            
            if ((second & 32) == (byte)32)
            {
                
                //有可恢复错误
                return new NormalErrorASB();
            }
            
            if ((second & 64) == (byte)64)
            {
               
                //有可自动恢复错误
                return new NormalErrorASB();
            }
            
            if ((third & 3) == (byte)3)
            {
                
                //打印纸将尽
                return new OutOfPaperASB();
            }
            
            if ((third & 12) == (byte)12)
            {
                
                //打印纸以用完
                return new OutOfPaperASB();
            }
            
            if ((first & 8) == (byte)8)
            {
                //打印机离线
                return new PrinterOffLineASB();
            }
            
            if ((third & 64) == (byte)64)
            {
                //打印机正在打印
                return new PrinterPrintingASB();
            }

            if ((forth & 64) == (byte)64)
            {
                //打印机出错禁止打印
                return new ForbidPrintASB();
            }
            if ((third & 32) == (byte)32)
            {
                //打印机正常任务未完成
                return new TaskUnfinishedASB();
            }
            else
            {
                //state.State += 1;
            }
            if ((third & 64) == (byte)0)
            {
                //state.State += 2;
                //if (state.State == 4)
                //{
                //    return new PrintOverASB();
                //}
                return new PrintOverASB();
            }
            return new PrinterNormal();
        }

    }
}
