using PreventLostCenter;
using PrintBase;
using SerialPortPrint;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace NetworkPrint.Win
{
    /// <summary>
    /// 发送打印数据TcpClient
    /// </summary>
    public class SendTcpClient : TcpClientBase
    {
        /// <summary>
        /// 打印机回调
        /// </summary>
        public Action<int> PrintExecCallBack;

        public SendTcpClient(string ip, int port, Action<Exception> socketErrorAction)
            : base(ip, port, socketErrorAction)
        {

        }
        protected override void BeginReceiveCallBack(IAsyncResult reault)
        {
            int cout = 0;
            try
            {
                cout = tcpClient.Client.EndReceive(reault);
            }
            catch (SocketException e)
            {
                socketError = e.SocketErrorCode;
            }
            catch (Exception ex)
            {
                socketError = SocketError.HostDown;
            }
            if (socketError == SocketError.Success && cout > 0)
            {
                byte[] buffer = reault.AsyncState as byte[];
                byte[] data = new byte[cout];
                Array.Copy(buffer, 0, data, 0, data.Length);
                PreventLostContext context = new PreventLostContext(data);
                PrinterState printerstate = context.Tactic(IP);
                if (PrintExecCallBack != null)
                {
                    PrintExecCallBack((int)printerstate);
                }
                BeginReceive();
            }
            else
            {
                if (tcpClient != null && tcpClient.Client != null)
                {
                    tcpClient.Client.Close();
                }
            }
        }
        /// <summary>
        /// 启动TCPClient
        /// </summary>
        /// <returns></returns>
        public bool StartTcpClient()
        {
            if (Connect() && StartStateReturn() == 0 && PrinterInite() == 0)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    BeginReceive();
                });
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 开启免丢单功能
        /// </summary>
        /// <returns></returns>
        private int StartPreventLost()
        {
            int error = 0;
            if (tcpClient.Client.Connected)
            {
                byte[] data = PrintCommand.StartPreventLost();
                try
                {
                    tcpClient.Client.Send(data);
                    error = 0;
                }
                catch (Exception ex)
                {
                    error = 2;
                }
            }
            return error;
        }
        /// <summary>
        ///开启打印机状态自动返回
        /// </summary>
        /// <returns></returns>
        private int StartStateReturn()
        {
            int error = 0;
            if (tcpClient.Client.Connected)
            {
                byte[] data = PrintCommand.StateReturn();
                try
                {
                    tcpClient.Client.Send(data);
                    //retcp.Client.Send(data);
                    error = 0;
                }
                catch (Exception ex)
                {
                    error = 2;
                }
            }
            return error;
        }
        /// <summary>
        /// 打印机初始化
        /// </summary>
        /// <returns></returns>
        public int PrinterInite()
        {
            int error = 0;
            if (tcpClient.Client.Connected)
            {
                byte[] data = PrintCommand.Inite();

                try
                {
                    tcpClient.Client.Send(data);
                    error = 0;

                }
                catch (Exception ex)
                {
                    error = 2;

                }
            }
            return error;
        }
        /// <summary>
        /// 打印机重置
        /// </summary>
        /// <returns></returns>
        public int PrinterReset()
        {
            int error = 0;
            if (tcpClient.Client.Connected)
            {
                byte[] data = PrintCommand.Reset();

                try
                {
                    tcpClient.Client.Send(data);
                    error = 0;

                }
                catch (Exception ex)
                {
                    error = 2;

                }
            }
            return error;
        }

        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public bool PrintImg(Bitmap bitmap, out int error, int pt)
        {
            if (tcpClient.Client.Connected)
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, 567, 0, (PrinterType)pt);

                try
                {
                    var res = tcpClient.Client.Send(data);

                    CutPage();

                    error = 0;
                    return true;
                }
                catch (Exception ex)
                {
                    error = 2;
                    if (SocketErrorAction != null)
                    {
                        SocketErrorAction(ex);
                    }
                    return false;
                }
            }

            error = 1;
            return false;

        }
        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="mess"></param> 
        /// <returns></returns>
        public int PrintString(string mess)
        {
            int error = 0;
            byte[] OutBuffer;//数据
            int BufferSize;
            Encoding targetEncoding;
            //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
            targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
            BufferSize = targetEncoding.GetByteCount(mess); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
            OutBuffer = new byte[BufferSize];
            OutBuffer = targetEncoding.GetBytes(mess);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码

            if (tcpClient.Client.Connected)
            {
                byte[] data = OutBuffer;

                try
                {
                    tcpClient.Client.Send(data);
                    CutPage();
                    error = 0;

                }
                catch (Exception ex)
                {
                    error = 2;
                    if (SocketErrorAction != null)
                    {
                        SocketErrorAction(ex);
                    }
                }
            }
            return error;
        }
        /// <summary>
        ///切纸
        /// </summary>
        private bool CutPage()
        {
            byte[] cmdData = new byte[6];

            cmdData[0] = 0x1B;
            cmdData[1] = 0x64;
            cmdData[2] = 0x05;
            cmdData[3] = 0x1D;
            cmdData[4] = 0x56;
            cmdData[5] = 0x00;

            try
            {
                var res=tcpClient.Client.Send(cmdData);
                return true;
            }
            catch (Exception ex)
            {
                if (SocketErrorAction != null)
                {
                    SocketErrorAction(ex);
                }
                return false;
            }
        }
    }
}
