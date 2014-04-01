using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

#if ANDROID

#else
using System.Drawing;
using PrintBase;
#endif

namespace NetworkPrint.Win
{
    public class PrintHelper
    {
        private SocketError socketError;
        private TcpClient tcp;
        private int port;
        private string ip;

        public PrintHelper(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            tcp = new TcpClient();
        }

        public bool Connect()
        {
            try
            {
                tcp.Connect(ip, port);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public bool IsOpen()
        {
            if (tcp != null)
            {
                return tcp.Client.Connected;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public bool PrintImg(Bitmap bitmap, out int error)
        {
            if (tcp.Client.Connected)
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, 567, 0);

                try
                {
                    tcp.Client.Send(data);
                    CutPage();
                    error = 0;
                    return true;
                }
                catch (Exception ex)
                {
                    error = 2;
                    return false;
                }
            }

            error = 1;
            return false;

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
                tcp.Client.Send(cmdData);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        

        private void BeginReceive()
        {
            byte[] data = new byte[1024];
            tcp.Client.BeginReceive(data, 0, data.Length, SocketFlags.None, out socketError, BeginReceiveCallBack, data);

        }

        private void BeginReceiveCallBack(IAsyncResult reault)
        {
            int cout = 0;
            try
            {
                cout = tcp.Client.EndReceive(reault);
            }
            catch (SocketException e)
            {
                socketError = e.SocketErrorCode;
            }
            catch
            {
                socketError = SocketError.HostDown;
            }
            if (socketError == SocketError.Success && cout > 0)
            {
                byte[] buffer = reault.AsyncState as byte[];
                byte[] data = new byte[cout];
                Array.Copy(buffer, 0, data, 0, data.Length);
                BeginReceive();
            }
            else
            {
                tcp.Client.Close();
            }
        }
    }
}
 