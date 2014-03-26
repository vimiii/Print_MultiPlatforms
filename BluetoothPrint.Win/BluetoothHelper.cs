using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using PrintBase;
using SerialPortPrint;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPrint.Win
{
    public class BluetoothHelper:IDisposable
    {
        private bool disposed = false;
        /// <summary>
        /// 搜索结束回调
        /// </summary>
        private BluetoothClient blueClient;
        Dictionary<string, BluetoothAddress> deviceAddress = new Dictionary<string, BluetoothAddress>();

        public void Inite()
        {
            blueClient = new BluetoothClient();
        }
        /// <summary>
        /// 蓝牙连接状态
        /// </summary>
        /// <returns></returns>
        public int IsConnected()
        {
            int err = 0;
            if (blueClient != null)
            {
                if (blueClient.Connected)
                {
                    err = 1;
                }
                else
                {
                    err = (int)PrintError.ConnectedFailure;
                }
            }
            return err;
        }

        /// <summary>
        /// 异步扫描蓝牙设备
        /// </summary>
        public void ScanAsync(Action<Dictionary<string, BluetoothAddress>> ScanOver)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    BluetoothRadio BuleRadio = BluetoothRadio.PrimaryRadio;
                    BuleRadio.Mode = RadioMode.Connectable;
                    BluetoothDeviceInfo[] Devices = blueClient.DiscoverDevices();
                   
                    deviceAddress.Clear();
                    foreach (BluetoothDeviceInfo device in Devices)
                    {
                        if (!deviceAddress.Keys.Contains(device.DeviceName))
                        {
                            deviceAddress[device.DeviceName] = device.DeviceAddress;
                        }
                    }
                    if (ScanOver != null)
                    {
                        ScanOver(deviceAddress);
                    }
                }
                catch(Exception ex)
                {
                    if (ScanOver != null)
                    {
                        ScanOver(null);
                    }
                }
                
            });
        }
        /// <summary>
        /// 连接蓝牙
        /// </summary>
        /// <param name="address">蓝牙设备地址</param>
        /// <param name="pwd">连接密码</param>
        /// <returns></returns>
        public int Connect(BluetoothAddress address, string pwd)
        {
            int err = 0;
            try
            {
                blueClient.SetPin(address, pwd);
                blueClient.Connect(address, BluetoothService.SerialPort);
                err = 1;
                return err;
            }
            catch(Exception ex)
            {
                err = (int)PrintError.ConnectedFailure;
                return err;
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            blueClient.Close();
        }
        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        public int PrintString(string mess)
        {
            int err = 0;
            if (blueClient.Connected)
            {
                byte[] OutBuffer;//数据
                int BufferSize;
                Encoding targetEncoding;
                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(mess); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(mess);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码
                int res= blueClient.Client.Send(OutBuffer);

                if (res == BufferSize)
                {
                    err = 1;
                }
                else
                {
                    err = (int)PrintError.SendFailure;
                }
            }
            else
            {
                err =(int)PrintError.NotConnectedBluetooth;
                
            }
            return err;
        }
        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public int PrintImg(Bitmap bitmap, int dpiWidth)
        {
            int err = 0;
            if (blueClient.Connected)
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, dpiWidth, 0);
                int res= blueClient.Client.Send(data);
                if (res == data.Length)
                {
                    err = 1;
                }
                else
                {
                    err = (int)PrintError.SendFailure;
                }
            }
            else
            {
                err = (int)PrintError.NotConnectedBluetooth;
            }
            return err;
        }

        /// <summary>
        ///切纸
        /// </summary>
        public int CutPage()
        {
            int err=0;
            byte[] cmdData = PrintCommand.Cut();
            int res= blueClient.Client.Send(cmdData);
            if (res == cmdData.Length)
            {
                err = 1;
            }
            else
            {
                err = (int)PrintError.SendFailure;
            }
            return err;
        }
        /// <summary>
        /// 走纸
        /// </summary>
        /// <param name="row"></param>
        public int WalkPaper(int row)
        {
            int err = 0;
            byte[] cmdData = PrintCommand.WalkPaper(row);
            int res= blueClient.Client.Send(cmdData);
            if( res == cmdData.Length)
            {
                err = 1;
            }
            else
            {
                err = (int)PrintError.SendFailure;
            }
            return err;
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.Dispose();
            }
            if (blueClient != null)
                blueClient.Close();
            disposed = true;
        }
        ~BluetoothHelper()
        {
            Dispose(false);
        }
    }
}
