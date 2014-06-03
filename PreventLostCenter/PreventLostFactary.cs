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
                state.State += 1;
                //打印机上盖打开
                return new CoverOpenASB();
            }
            else
            {
                state.State = 1;
            }
            if ((second & 8) == (byte)8)
            {
                state.State += 2;
                //有切刀错误
                return new CutterErrorrASB();
            }
            else
            {
                state.State = 1;
            }
            if ((second & 32) == (byte)32)
            {
                state.State +=3;
                //有可恢复错误
                return new NormalErrorASB();
            }
            else
            {
                state.State = 1;
            }
            if ((second & 64) == (byte)64)
            {
                state.State += 4;
                //有可自动恢复错误
                return new NormalErrorASB();
            }
            else
            {
                state.State = 1;
            }
            if ((third & 3) == (byte)3)
            {
                state.State += 5;
                //打印纸将尽
                return new OutOfPaperASB();
            }
            else
            {
                state.State = 1;
            }
            if ((third & 12) == (byte)12)
            {
                state.State += 6;
                //打印纸以用完
                return new OutOfPaperASB();
            }
            else
            {
                state.State = 1;

            }
            if ((first & 8) == (byte)8)
            {
                state.State += 7;
                //打印机离线
                return new PrinterOffLineASB();
            }
            else
            {
                state.State = 1;
            }
            if ((third & 64) == (byte)64)
            {
                //打印机正在打印
                state.State += 8;
                return new PrinterPrintingASB();
            }

            if ((forth & 64) == (byte)64)
            {
                state.State += 9;
                //打印机出错禁止打印
                return new ForbidPrintASB();
            }
            else
            {
                state.State = 1;
            }
            if ((third & 32) == (byte)32)
            {
                state.State += 10;
                //打印机正常任务未完成
                return new TaskUnfinishedASB();
            }
            else
            {
                state.State = 1;
            }

            if ((third & 32) == (byte)0)
            {
                state.State += 11;
                if (state.State == 20)
                {
                    state.State = 1;
                    return new PrintOverASB();
                }
                else
                {
                    state.State = 1;
                    return new PrinterNormal();
                }
            }
            state.State = 1;
            return new PrinterNormal();
        }

    }
}
