using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialPortPrint
{
    /// <summary>
    /// 打印机指令
    /// </summary>
    public static class PrintCommand
    {
        /// <summary>
        /// 打印机走纸
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static byte[] WalkPaper(int row)
        {
            string cmd = "0x" + row.ToString();
            byte[] cmdData = new byte[3];
            //走纸
            cmdData[0] = 0x1B;
            cmdData[1] = 0x64;
            //cmdData[2] = 0x04;
            cmdData[2] = (byte)row;
            return cmdData;
        }
        /// <summary>
        /// 打印机切纸
        /// </summary>
        /// <returns></returns>
        public static byte[] Cut()
        {
            byte[] cmdData = new byte[5];

            //切纸
            cmdData[0] = 0x1B;
            cmdData[1] = 0x40;
            cmdData[2] = 0x1D;
            cmdData[3] = 0x56;
            cmdData[4] = 0x00;
            return cmdData;
        }
        /// <summary>
        /// 换行
        /// </summary>
        public static byte[] CRLF()
        {
            byte[] cmdData = new byte[2];

            //换行
            cmdData[0] = 0x0d;
            cmdData[1] = 0x0a;
            return cmdData;
        }
        /// <summary>
        /// 打印机初始化状态
        /// </summary>
        /// <returns></returns>
        public static byte[] Reset()
        {
            byte[] cmdData = new byte[3];
            cmdData[0] = 0x1B;
            cmdData[1] = 0x41;
            cmdData[2] = 0x00;
            return cmdData;
        }

        /// <summary>
        /// 请求打印机状态
        /// </summary>
        /// <returns></returns>
        public static byte[] RequestPrinterState()
        {
            byte[] cmdData = new byte[3];


            cmdData[0] = 0x1B;
            cmdData[1] = 0x76;
            cmdData[2] = 0x01;
            return cmdData;
        }


        #region 免丢单打印机指令
        /// <summary>
        ///开启打印机免丢单功能
        /// </summary>
        /// <returns></returns>
        public static byte[] StartPreventLost()
        {
            byte[] cmdData = new byte[10];

            cmdData[0] = 0x1B;
            cmdData[1] = 0x73;
            cmdData[2] = 0x42;
            cmdData[3] = 0x45;
            cmdData[4] = 0x92;
            cmdData[5] = 0x9A;
            cmdData[6] = 0x01;
            cmdData[7] = 0x00;
            cmdData[8] = 0x5F;
            cmdData[9] = 0x0A;
            return cmdData;
        }
        /// <summary>
        /// 打印机状态自动返回
        /// </summary>
        /// <returns></returns>
        public static byte[] StateReturn()
        {
            byte[] cmdData = new byte[3];
            cmdData[0] = 0x1D;
            cmdData[1] = 0x61;
            cmdData[2] = 0x0E;
            return cmdData;
        }
        /// <summary>
        /// 主动请求打印机状态
        /// </summary>
        /// <returns></returns>
        public static byte[] GetState()
        {
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x1B;
            cmdData[1] = 0x76;
            return cmdData;
        }
        /// <summary>
        /// 打印机初始化状态
        /// </summary>
        /// <returns></returns>
        public static byte[] Inite()
        {
            byte[] cmdData = new byte[2];
            cmdData[0] = 0x1B;
            cmdData[1] = 0x40;
            return cmdData;
        }
        /// <summary>
        /// 清除打印机状态
        /// </summary>
        /// <returns></returns>
        public static byte[] ClearState()
        {
            byte[] cmdData = new byte[5];
            cmdData[0] = 0x10;
            cmdData[1] = 0x06;
            cmdData[2] = 0x07;
            cmdData[3] = 0x08;
            cmdData[4] = 0x01;
            return cmdData;

        }
        #endregion
    }
}
