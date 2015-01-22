using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;

using Android.Hardware.Usb;
using Android.Graphics;
using Android.Content;
using Android.Util;
using PrintBase;
using SerialPortPrint;

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
        List<int> productList = new List<int>();
        public PrintHelper()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">当前context</param>
        /// <param name="vid">打印机vid</param>
        /// <param name="pid">打印机pid</param>
        /// <param name="dpiWidth">打印机每行dpi点数</param>
        /// <param name="err">异常</param>
        /// <returns></returns>
        public int Inite(Context context, int vid, int pid)
        {
            int err = 0;
            // 获取UsbManager
            myUsbManager = (UsbManager)context.GetSystemService(UsbService);

            if (!enumerateDevice(vid, pid))
            {
                err = (int)PrintError.NotFindDevice;//未找到设备
                return err;
            }

            findInterface();

            assignEndpoint();
            return openDevice();
        }
        public int IsOpen()
        {
            int err = 0;
            if (myDeviceConnection == null)
            {
                err = (int)PrintError.ConnectedFailure;
                return err;
            }
            else
            {
                err = 1;
                return err;
            }
        }
        public int Open()
        {
            int err = 0;
            err= openDevice();
            return err;
        }
       
        /// <summary>
        /// 释放连接
        /// </summary>
        public void Close()
        {

            if (myDeviceConnection != null)
            {
                myDeviceConnection.Close();
                myDeviceConnection = null;
            }
        }
        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        public int PrintString(string mess)
        {
            int err = 0;
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

                if (sendPackage(OutBuffer))
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
                err = (int)PrintError.SendNull;
            }
            return err;

        }
        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public int PrintImg(Bitmap bitmap, int dpiWidth,int pt)
        {
            int err = 0;

            if (bitmap != null)
            {
                try
                {
                    byte[] data = Pos.POS_PrintPicture(bitmap, dpiWidth, 0, (PrinterType)pt);
                    if (data != null)
                    {
                        if (sendPackage(data))
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
                        err = (int)PrintError.PosCMDErr;
                    }
                }
                catch
                {
                    err = (int)PrintError.PosCMDErr;
                }
            }
            else
            {
                err = (int)PrintError.SendNull;
            }
            return err;
        }
        /// <summary>
        /// 换行
        /// </summary>
        /// <returns></returns>
        public int CRLF()
        {
            int err = 0;
            byte[] cmdData = PrintCommand.CRLF();
            if (sendPackage(cmdData))
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
        ///切纸
        /// </summary>
        public int CutPage()
        {
            int err = 0;
            byte[] cmdData = PrintCommand.Cut();
            if (sendPackage(cmdData))
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
            if (sendPackage(cmdData))
            {
                err = 1;
            }
            else
            {
                err = (int)PrintError.SendFailure;
            }
            return err;
        }
      
        public int Reset()
        {
            int err = 0;
            byte[] cmdData = PrintCommand.Reset();
            if (sendPackage(cmdData))
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
        /// 请求打印机状态
        /// </summary>
        /// <returns></returns>
        public int PrinterState()
        {
            int err = 0;
            byte[] cmdData = PrintCommand.RequestPrinterState();
            if (sendCommand(cmdData)==1)
            {
                err = 1;
            }
            else
            {
                err = (int)PrintError.SendFailure;
            }
            return err;
        }

        /**
  * 分配端点，IN | OUT，即输入输出；此处我直接用1为OUT端点，0为IN，当然你也可以通过判断
  */
        private void assignEndpoint()
        {
            /*
            string mess = "1111111111111111111111111111111111";
            byte[] OutBuffer;//数据
            int BufferSize;
            Encoding targetEncoding;
            //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
            targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
            BufferSize = targetEncoding.GetByteCount(mess); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
            OutBuffer = new byte[BufferSize];
            OutBuffer = targetEncoding.GetBytes(mess);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码
            */
            try
            {
                for (int i = 0; i < myInterface.EndpointCount; i++)
                {
                    try
                    {
                        UsbEndpoint point = myInterface.GetEndpoint(i);
                        //int res = myDeviceConnection.BulkTransfer(point, OutBuffer, BufferSize, 10000);
                        if (point.Direction == UsbAddressing.Out)
                        {
                            epOut = point;
                        }
                        else //if (point.Direction == UsbAddressing.In)
                        {
                            epIn = point;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }

        /**
         * 打开设备
         */
        private int openDevice()
        {
            int err = 0;
            if (myInterface != null)
            {
                UsbDeviceConnection conn = null;
                // 在open前判断是否有连接权限；对于连接权限可以静态分配，也可以动态分配权限，可以查阅相关资料
                // conn = myUsbManager.OpenDevice(myUsbDevice);
                if (myUsbManager.HasPermission(myUsbDevice))
                {
                    conn = myUsbManager.OpenDevice(myUsbDevice);
                }
                else
                {
                    err = (int)PrintError.NoPermission;
                    Log.Debug("err", "没有连接权限");
                    return err;
                }

                if (conn.ClaimInterface(myInterface, true))
                {
                    myDeviceConnection = conn; // 到此你的android设备已经连上HID设备
                    Log.Debug("info", "连接HID设备成功");
                    err = 1;
                    return err;
                }
                else
                {
                    err = (int)PrintError.ConnectedFailure;
                    Log.Debug("err", "连接HID设备失败");
                    conn.Close();
                    return err;
                }
            }
            else
            {
                err = (int)PrintError.Error;
                return err;
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
                    myInterface = intf;
                    //openDevice();
                    //assignEndpoint();

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
                    //break;
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
        private bool sendPackage(byte[] command)
        {
            if (myDeviceConnection==null)
            {
                return false;
            }
            int len = command.Length;

            //分批发送2
            int packageLength = 1024;
            Dictionary<int, byte[]> data = new System.Collections.Generic.Dictionary<int, byte[]>();
            int num = len / packageLength + 1;
            for (int i = 0; i < num; i++)
            {
                byte[] da = new byte[packageLength];
                for (int j = 0; j < packageLength; j++)
                {
                    int index = i * packageLength + j;
                    if (index >= len)
                    {
                        break;
                    }
                    da[j] = command[index];
                }
                data.Add(i, da);
            }

            foreach (KeyValuePair<int,byte[]> kvp in data)
            {
                int res = myDeviceConnection.BulkTransfer(epOut, kvp.Value, kvp.Value.Length, 10000);
                System.Threading.Thread.Sleep(5);
                if (res != kvp.Value.Length)
                {
                    return false;
                }
            }
            return true;
        }

        private int sendCommand(byte[] command) {
            int err = 0;
            if (myDeviceConnection == null)
            {
                err = (int)PrintError.ConnectedFailure;
                return err;
            }
            int len = command.Length;
            int res = myDeviceConnection.BulkTransfer(epOut, command, len, 10000);
            if (res != len)
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
            byte[] reState = new byte[1] ;
            int ret = myDeviceConnection.BulkTransfer(epIn, reState, 1, 10000);
            if (ret != 13)
            {
                
            }


            return err;
            

        }

    }
}
