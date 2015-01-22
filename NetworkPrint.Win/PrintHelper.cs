using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

#if ANDROID

#else
using System.Drawing;
using PrintBase;
using SerialPortPrint;
using PreventLostCenter;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
#endif

namespace NetworkPrint.Win
{
    public class PrintHelper
    {
        private static bool IsConnectionSuccessful = false;
        private static ManualResetEvent connectedTimeoutObject;
        private int _tcpHeartRate = 5 * 60 * 1000;
        private SocketError socketError;
        private int Sendport = 9100;  //发送打印命令端口
        private int Stateport = 4000;//状态监测命令端口
        private TcpClient sendTcp;
        private TcpClient stateTcp;
        private string ip;
        /// <summary>
        /// 打印机回调
        /// </summary>
        public Action<int> PrintExecCallBack;
        /// <summary>
        /// SocketError异常回调
        /// </summary>
        public Action<Exception> SocketErrorAction;

        public PrintHelper(string ip, Action<Exception> SocketErrorAction)
        {
            this.ip = ip;
            this.SocketErrorAction = SocketErrorAction;
            connectedTimeoutObject = new ManualResetEvent(false);
            sendTcp = new TcpClient();
        }

        public bool Connect()
        {
            try
            {
                //tcp = new TcpClient();
                //tcp.Connect(ip, Sendport);
                //return true;
                return Connect(ip, Sendport);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 连接Server
        /// </summary>
        /// <param name="serverHostname"></param>
        /// <param name="serverIPPoint"></param>
        private bool Connect(string serverHostname, int serverIPPoint, int timeoutMiliSecond = 500)
        {
            connectedTimeoutObject.Reset();

            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)_tcpHeartRate).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)_tcpHeartRate).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

            sendTcp.Client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

            sendTcp.BeginConnect(serverHostname, serverIPPoint, new AsyncCallback(ConnectCallBack), sendTcp);

            if (connectedTimeoutObject.WaitOne(timeoutMiliSecond, false))
            {
                if (IsConnectionSuccessful)
                {
                    return true;
                }
                else
                {
                    //throw socketError;
                    return false;
                }
            }
            else
            {
                try
                {
                    //_tcpClient.Client.Shutdown(SocketShutdown.Both);

                    if (sendTcp.Client != null)
                    {
                        sendTcp.Client.Close(3000);
                    }
                    sendTcp.Close();
                }
                catch (Exception ex)
                {

                }
                throw new TimeoutException("TimeOut Exception");
            }
        }

        private void ConnectCallBack(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
            }
            finally
            {
                connectedTimeoutObject.Set();
            }
        }

        public bool Close()
        {
            sendTcp.Close();
            sendTcp = null;
            return true;
        }

        public bool IsOpen()
        {
            if (sendTcp != null)
            {
                return sendTcp.Client.Connected;
            }
            else
            {
                return false;
            }
        }

        public int StartPreventLost()
        {
            int error = 0;
            if (sendTcp.Client.Connected)
            {
                byte[] data = PrintCommand.StartPreventLost();

                try
                {
                    sendTcp.Client.Send(data);
                    error = 0;

                }
                catch (Exception ex)
                {
                    error = 2;
                }
            }
            return error;
        }

        public int StartStateReturn()
        {
            int error = 0;
            if (sendTcp.Client.Connected)
            {
                byte[] data = PrintCommand.StateReturn();

                try
                {
                    sendTcp.Client.Send(data);
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
        public int GetState()
        {
            int error = 0;
            if (sendTcp.Client.Connected)
            {
                byte[] data = PrintCommand.GetState();

                try
                {
                    sendTcp.Client.Send(data);
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
        public int PrinterInite()
        {
            int error = 0;
            if (sendTcp.Client.Connected)
            {
                byte[] data = PrintCommand.Inite();

                try
                {
                    sendTcp.Client.Send(data);
                    error = 0;

                }
                catch (Exception ex)
                {
                    error = 2;

                }
            }
            return error;
        }
        public int ClearState()
        {
            int error = 0;
            if (sendTcp.Client.Connected)
            {
                byte[] data = PrintCommand.ClearState();

                try
                {
                    sendTcp.Client.Send(data);
                    error = 0;

                }
                catch (Exception ex)
                {
                    error = 2;

                }
            }
            return error;
        }

        public void BeginReceive()
        {
            try
            {
                byte[] data = new byte[4];
                sendTcp.Client.BeginReceive(data, 0, data.Length, SocketFlags.None, out socketError, BeginReceiveCallBack, data);
            }
            catch (Exception ex)
            {
                if (SocketErrorAction != null)
                {
                    SocketErrorAction(ex);
                }
            }
        }
        private void BeginReceiveCallBack(IAsyncResult reault)
        {
            int cout = 0;
            try
            {
                cout = sendTcp.Client.EndReceive(reault);
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
                PrinterState printerstate = context.Tactic(ip);
                if (PrintExecCallBack != null)
                {
                    PrintExecCallBack((int)printerstate);
                }
                BeginReceive();
            }
            else
            {
                sendTcp.Client.Close();
            }
        }



        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public bool PrintImg(Bitmap bitmap, out int error, int pt)
        {
            if (sendTcp.Client.Connected)
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, 567, 0, (PrinterType)pt);

                try
                {
                    sendTcp.Client.Send(data);
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

            if (sendTcp.Client.Connected)
            {
                byte[] data = OutBuffer;

                try
                {
                    sendTcp.Client.Send(data);
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
                sendTcp.Client.Send(cmdData);
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
