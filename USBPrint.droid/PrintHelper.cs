using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;

using Android.Hardware.Usb;
using Android.Graphics;
using Android.Content;
using Android.Util;
using PrintBase;

namespace USBPrint.droid
{
    public class PrintHelper
    {
        public const string UsbService = "usb";
        private UsbManager myUsbManager;   //USB管理器  
        private UsbDevice myUsbDevice;  //找到的USB设备  
        private UsbInterface myInterface;
        private UsbEndpoint epOut;
        private UsbEndpoint epIn;
        UsbDeviceConnection myDeviceConnection = null;
        private int dpiWidth;
        List<int> productList = new List<int>();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">当前context</param>
        /// <param name="vid">打印机vid</param>
        /// <param name="pid">打印机pid</param>
        /// <param name="dpiWidth">打印机每行dpi点数</param>
        /// <param name="err">异常</param>
        /// <returns></returns>
        public bool Inite(Context context, int vid, int pid,int dpiWidth, out int err)
        {
            err = 0;
            this.dpiWidth = dpiWidth;
            // 获取UsbManager
            myUsbManager = (UsbManager)context.GetSystemService(UsbService);

            if (!enumerateDevice(vid, pid))
            {
                err = 1;//未找到设备
                return false;
            }

            findInterface();

            assignEndpoint();

            return true;
        }
        public bool IsOpen()
        {
            if (myDeviceConnection != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Open()
        {
            if (openDevice())
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 释放连接
        /// </summary>
        public void Close()
        {
            myDeviceConnection.Close();
        }
        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        public bool PrintString(string mess, out int error)
        {
            error = 0;
            if (!String.IsNullOrEmpty(mess))
            {
                byte[] OutBuffer;//数据
                int BufferSize;
                Encoding targetEncoding;
                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(mess); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(mess);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码


                sendPackage(OutBuffer);
                CutPage();
                return true;
            }
            else
            {
                error = 10;
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
            error = 0;

            if (bitmap != null)
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, dpiWidth, 0);
                sendPackage(data);
                CutPage();
                return true;
            }
            else
            {
                error = 10;
                return false;
            }
        }
        /// <summary>
        ///切纸
        /// </summary>
        private void CutPage()
        {
            byte[] cmdData = new byte[8];

            cmdData[0] = 0x1B;
            cmdData[1] = 0x64;
            cmdData[2] = 0x04;
            cmdData[3] = 0x1B;
            cmdData[4] = 0x40;
            cmdData[5] = 0x1D;
            cmdData[6] = 0x56;
            cmdData[7] = 0x00;

            sendPackage(cmdData);
        }

        /**
  * 分配端点，IN | OUT，即输入输出；此处我直接用1为OUT端点，0为IN，当然你也可以通过判断
  */
        private void assignEndpoint()
        {
            try
            {
                for (int i = 0; i < myInterface.EndpointCount; i++)
                {
                    UsbEndpoint point = myInterface.GetEndpoint(i);
                    if (point.Direction == UsbAddressing.Out)
                    {
                        epOut = point;
                    }
                    else
                    {
                        epIn = point;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /**
         * 打开设备
         */
        private bool openDevice()
        {
            if (myInterface != null)
            {
                UsbDeviceConnection conn = null;
                // 在open前判断是否有连接权限；对于连接权限可以静态分配，也可以动态分配权限，可以查阅相关资料
                if (myUsbManager.HasPermission(myUsbDevice))
                {
                    conn = myUsbManager.OpenDevice(myUsbDevice);
                }
                else
                {
                    Log.Debug("err", "没有连接权限");
                    return false;
                }

                if (conn.ClaimInterface(myInterface, true))
                {
                    myDeviceConnection = conn; // 到此你的android设备已经连上HID设备
                    Log.Debug("info", "连接HID设备成功");
                    return true;
                }
                else
                {
                    Log.Debug("err", "连接HID设备失败");
                    conn.Close();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /**
         * 找设备接口
         */
        private void findInterface()
        {
            if (myUsbDevice != null)
            {
                for (int i = 0; i < myUsbDevice.InterfaceCount; i++)
                {
                    UsbInterface intf = myUsbDevice.GetInterface(i);
                    if (intf.EndpointCount >= 2)
                    {
                        myInterface = intf;
                        break;
                    }
                }
            }
        }

        /**
         * 枚举设备
         */
        private bool enumerateDevice(int vid, int pid)
        {
            if (myUsbManager == null)
                return false;
            IEnumerator<KeyValuePair<string, UsbDevice>> usbList = myUsbManager.DeviceList.GetEnumerator();

            // UsbDevice mUsbDevice = null;
            while (usbList.MoveNext())
            {
                KeyValuePair<string, UsbDevice> kv = usbList.Current;
                productList.Add(kv.Value.ProductId);
                if (kv.Value.ProductId == pid && kv.Value.VendorId == vid)
                {
                    myUsbDevice = kv.Value;
                    break;
                }
            }
            if (myUsbDevice == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="command"></param>
        private void sendPackage(byte[] command)
        {
            int len = command.Length;
            
            //分批发送
            int packageLength = 1000;
            Dictionary<int, byte[]> data = new System.Collections.Generic.Dictionary<int, byte[]>();
            int num = len / packageLength + 1;
            for (int i = 0; i < num; i++)
            {
                byte[] da = new byte[packageLength];
                for (int j = 0; j < packageLength; j++)
                {
                    int index = i * packageLength + j;
                    if (index>=len)
                    {
                        break;
                    }
                    da[j] = command[index];
                }
                data.Add(i, da);
            }

            foreach (int i in data.Keys)
            {
                myDeviceConnection.BulkTransfer(epOut, data[i], data[i].Length, 10000);
            }
        }

    }
}
