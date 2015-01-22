using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NetworkPrint.Win
{
    /// <summary>
    /// tcpClient 基础类
    /// </summary>
    public class TcpClientBase
    {
        protected static bool IsConnectionSuccessful = false;
        protected static ManualResetEvent connectedTimeoutObject;
        protected int _tcpHeartRate = 5 * 60 * 1000;
        protected SocketError socketError;
        //protected int sendport = 9100;  //发送打印命令端口
        //protected int stateport = 4000;//状态监测命令端口
        protected string IP;
        protected int Port;
        public TcpClient tcpClient { get; set; }
        /// <summary>
        /// SocketError异常回调
        /// </summary>
        public Action<Exception> SocketErrorAction;
        public TcpClientBase(string IP, int Port, Action<Exception> SocketErrorAction)
        {
            this.IP = IP;
            this.Port = Port;
            this.SocketErrorAction = SocketErrorAction;
            connectedTimeoutObject = new ManualResetEvent(false);
            tcpClient = new TcpClient();
        }

        protected bool Connect()
        {
            try
            {
                //tcp = new TcpClient();
                //tcp.Connect(ip, Sendport);
                //return true;
                return Connect(IP, Port);
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

            tcpClient.Client.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);

            tcpClient.BeginConnect(serverHostname, serverIPPoint, new AsyncCallback(ConnectCallBack), tcpClient);

            if (connectedTimeoutObject.WaitOne(timeoutMiliSecond, false))
            {
                if (IsConnectionSuccessful)
                {
                    BeginReceive();
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

                    if (tcpClient.Client != null)
                    {
                        tcpClient.Client.Close(3000);
                    }
                    tcpClient.Close();
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
            if (tcpClient != null)
            {
                tcpClient.Close();
                //tcpClient = null;
            }
            return true;
        }

        public bool IsOpen()
        {
            if (tcpClient != null)
            {
                return tcpClient.Client.Connected;
            }
            else
            {
                return false;
            }
        }
        protected void BeginReceive()
        {
            try
            {
                byte[] data = new byte[4];
                tcpClient.Client.BeginReceive(data, 0, data.Length, SocketFlags.None, out socketError, BeginReceiveCallBack, data);
            }
            catch (Exception ex)
            {
                if (SocketErrorAction != null)
                {
                    SocketErrorAction(ex);
                }
            }
        }
        protected virtual void BeginReceiveCallBack(IAsyncResult reault)
        {
        
        }
        
    }
}
