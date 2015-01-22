using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PreventLostCenter
{
    /// <summary>
    /// 打印机状态
    /// </summary>
    public enum PrinterState
    {
        /// <summary>
        /// 没有处理该打印机状态
        /// </summary>
        UnCatchPrinter=0,
        /// <summary>
        /// 打印机正常
        /// </summary>
        PrinterNormal=1,
        /// <summary>
        /// 打印机正在打印
        /// </summary>
        PrinterPrinting,
        /// <summary>
        /// 一次打印结束
        /// </summary>
        PrintOver,
        /// <summary>
        /// 打印机上盖打开
        /// </summary>
        CoverOpen,
        /// <summary>
        /// 切刀错误
        /// </summary>
        CutterErrorr,
        /// <summary>
        /// 打印机禁止打印
        /// </summary>
        ForbidPrint,
        /// <summary>
        /// 打印机有可恢复异常
        /// </summary>
        NormalError,
        /// <summary>
        /// 打印机缺纸
        /// </summary>
        OutOfPaper,
        /// <summary>
        /// 打印机离线
        /// </summary>
        PrinterOffLine,
        /// <summary>
        /// 打印机正常任务未完成
        /// </summary>
        TaskUnfinished,

    }
}
