using LibUsbDotNet;
using LibUsbDotNet.Main;
using PrintBase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBPrint.Win
{
    public class PrintHelper
    {
        private UsbDevice MyUsbDevice;

        private UsbDeviceFinder MyUsbFinder;
        private UsbEndpointWriter Writer;
        private UsbEndpointReader Reader;
        private int dpiWidth;

        public void Inite(int vid, int pid,int dpiWidth)
        {
            this.dpiWidth = dpiWidth;
            MyUsbFinder = new UsbDeviceFinder(1659, 8965);
        }
        public bool IsOpen()
        {
            if (MyUsbDevice != null)
            {
                return MyUsbDevice.IsOpen;
            }
            else
            {
                return false;
            }
        }
        public bool Open()
        {
            try
            {
                if (MyUsbFinder != null)
                {
                    MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);
                    if (MyUsbDevice == null)
                    {
                        return false;
                    }
                    IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // This is a "whole" USB device. Before it can be used, 
                        // the desired configuration and interface must be selected.

                        // Select config #1
                        wholeUsbDevice.SetConfiguration(1);

                        // Claim interface #0.
                        wholeUsbDevice.ClaimInterface(0);
                    }
                    Writer = MyUsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
                    Reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 关闭连接释放USB
        /// </summary>
        public void Close()
        {
            if (MyUsbDevice != null)
            {
                if (MyUsbDevice.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = MyUsbDevice as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        // Release interface #0.
                        wholeUsbDevice.ReleaseInterface(0);
                    }

                    MyUsbDevice.Close();
                }
                MyUsbDevice = null;

                // Free usb resources
                UsbDevice.Exit();
            }
        }
        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        public bool PrintString(string mess, out int error)
        {
            error = 0;
            ErrorCode ec = ErrorCode.None;
            error = 0;
            if (!String.IsNullOrEmpty(mess) && MyUsbDevice.IsOpen)
            {
                byte[] OutBuffer;//数据
                int BufferSize;
                Encoding targetEncoding;
                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(mess); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(mess);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码

                int bytesWritten;
                ec = Writer.Write(OutBuffer, 2000, out bytesWritten);
                if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

                byte[] readBuffer = new byte[1024];
                while (ec == ErrorCode.None)
                {
                    int bytesRead;

                    // If the device hasn't sent data in the last 100 milliseconds,
                    // a timeout error (ec = IoTimedOut) will occur. 
                    ec = Reader.Read(readBuffer, 100, out bytesRead);

                    if (bytesRead == 0) { }
                }
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
            ErrorCode ec = ErrorCode.None;
            if (bitmap!=null && MyUsbDevice.IsOpen)
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, dpiWidth, 0);
                int bytesWritten;
                ec = Writer.Write(data, 2000, out bytesWritten);
                if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

                byte[] readBuffer = new byte[1024];
                while (ec == ErrorCode.None)
                {
                    int bytesRead;

                    // If the device hasn't sent data in the last 100 milliseconds,
                    // a timeout error (ec = IoTimedOut) will occur. 
                    ec = Reader.Read(readBuffer, 100, out bytesRead);

                    if (bytesRead == 0) { }
                }
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
            ErrorCode ec = ErrorCode.None;
            byte[] cmdData = new byte[8];

            cmdData[0] = 0x1B;
            cmdData[1] = 0x64;
            cmdData[2] = 0x04;
            cmdData[3] = 0x1B;
            cmdData[4] = 0x40;
            cmdData[5] = 0x1D;
            cmdData[6] = 0x56;
            cmdData[7] = 0x00;


            int bytesWritten;
            ec = Writer.Write(cmdData, 2000, out bytesWritten);
            if (ec != ErrorCode.None) throw new Exception(UsbDevice.LastErrorString);

            byte[] readBuffer = new byte[1024];
            while (ec == ErrorCode.None)
            {
                int bytesRead;

                // If the device hasn't sent data in the last 100 milliseconds,
                // a timeout error (ec = IoTimedOut) will occur. 
                ec = Reader.Read(readBuffer, 100, out bytesRead);

                if (bytesRead == 0) { }
            }
        }

    }
}
