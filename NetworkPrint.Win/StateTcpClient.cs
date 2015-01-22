using PreventLostCenter;
using SerialPortPrint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace NetworkPrint.Win
{
    /// <summary>
    /// 状态请求TcpClient
    /// </summary>
    public class StateTcpClient : TcpClientBase
    {
        /// <summary>
        /// 打印机回调
        /// </summary>
        public Action<int> PrintExecCallBack;
        public StateTcpClient(string ip, int port, Action<Exception> socketErrorAction)
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
                if (tcpClient != null && tcpClient.Client!=null)
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
            if (Connect())
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
        /// 主动请求打印机状态
        /// </summary>
        /// <returns></returns>
        public int GetState()
        {
            int error = 0;
            if (tcpClient.Client.Connected)
            {
                byte[] data = PrintCommand.GetState();

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
    }
}
