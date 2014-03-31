using LibUsbDotNet;
using LibUsbDotNet.Main;
using PrintBase;
using SerialPortPrint;
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

        public int Inite(int vid, int pid)
        {
            MyUsbFinder = new UsbDeviceFinder(vid, pid);
            return Open();
        }
        public int IsOpen()
        {
            int err=0;
            if (MyUsbDevice != null)
            {
                if (MyUsbDevice.IsOpen)
                {
                    err = 1;
                }
                else
                {
                    err = (int)PrintError.OpenFailure;
                }
            }
            else
            {
                err = (int)PrintError.NotFindDevice;
            }
            return err;
        }
        public int Open()
        {
            int err = 0;
            try
            {
                if (MyUsbFinder != null)
                {
                    MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);
                    if (MyUsbDevice == null)
                    {
                        err = (int)PrintError.NotFindDevice;
                        return err;
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
                    //选择正确的EndPoint
                    int point = ChooseEndpoint(MyUsbDevice);
                    Writer = MyUsbDevice.OpenEndpointWriter((WriteEndpointID)point);
                    Reader = MyUsbDevice.OpenEndpointReader((ReadEndpointID)(point+128));
                    err = 1;
                    return err;
                }
                else
                {
                    err = (int)PrintError.Error;
                    return err;
                }

            }
            catch
            {
                err = (int)PrintError.Error;
                return err;
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
        public int PrintString(string mess)
        {
           int err =0;
            ErrorCode ec = ErrorCode.None;
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
                if (ec != ErrorCode.None) {
                    err = (int)PrintError.SendFailure;
                    return err;
                }
                else if (bytesWritten != OutBuffer.Length)
                {
                    err = (int)PrintError.SendFailure;
                    return err;
                }
                
                byte[] readBuffer = new byte[1024];
                while (ec == ErrorCode.None)
                {
                    int bytesRead;

                    // If the device hasn't sent data in the last 100 milliseconds,
                    // a timeout error (ec = IoTimedOut) will occur. 
                    ec = Reader.Read(readBuffer, 100, out bytesRead);

                    if (bytesRead == 0) { }
                }
                
                err = 1;
                return err;
            }
            else
            {
                err = (int)PrintError.SendNull;
                return err;
            }


        }
        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public int PrintImg(Bitmap bitmap, int dpiWidth)
        {
            int err = 0;
            ErrorCode ec = ErrorCode.None;
            if (bitmap!=null && MyUsbDevice.IsOpen)
            {
                try
                {
                    byte[] data = Pos.POS_PrintPicture(bitmap, dpiWidth, 0);
                    /*
                    byte[] cmdData = new byte[data.Length + 5];
                    cmdData[0] = 0x1B;
                    cmdData[1] = 0x2A;
                    cmdData[2] = 0x0;
                    cmdData[3] = 0x50;
                    cmdData[4] = 0x3;
                    for (int i = 0; i < data.Length; i++)
                    {
                        cmdData[6 + i] = data[i];
                    }
                    */
                    int bytesWritten;
                    ec = Writer.Write(data, 10000, out bytesWritten);
                    if (ec != ErrorCode.None)
                    {
                        err = (int)PrintError.SendFailure;
                        return err;
                    }
                    else if (bytesWritten != data.Length)
                    {
                        err = (int)PrintError.SendFailure;
                        return err;
                    }
                    err = 1;
                    return err;
                }
                catch
                {
                    err = (int)PrintError.PosCMDErr;
                    return err;
                }
            }
            else
            {
                err = (int)PrintError.SendNull;
                return err;
            }
        }
        /// <summary>
        ///切纸
        /// </summary>
        public int CutPage()
        {
            int err = 0;
            ErrorCode ec = ErrorCode.None;
            byte[] cmdData = SerialPortPrint.PrintCommand.Cut();

            int bytesWritten;
            ec = Writer.Write(cmdData, 2000, out bytesWritten);
            if (ec != ErrorCode.None) {
                err = (int)PrintError.SendFailure;
                return err;
            }
            else if (bytesWritten != cmdData.Length)
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
            err = 1;
            return err;
            /*
            byte[] readBuffer = new byte[1024];
            while (ec == ErrorCode.None)
            {
                int bytesRead;

                // If the device hasn't sent data in the last 100 milliseconds,
                // a timeout error (ec = IoTimedOut) will occur. 
                ec = Reader.Read(readBuffer, 100, out bytesRead);

                if (bytesRead == 0) { }
            }
             */
        }
        /// <summary>
        /// 走纸
        /// </summary>
        /// <param name="row"></param>
        public int WalkPaper(int row)
        {
            int err = 0;
            ErrorCode ec = ErrorCode.None;
            byte[] cmdData = PrintCommand.WalkPaper(row);
            int bytesWritten;
            ec = Writer.Write(cmdData, 2000, out bytesWritten);
            if (ec != ErrorCode.None)
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
            else if (bytesWritten != cmdData.Length)
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
            err = 1;
            return err;
        }

        /// <summary>
        /// 请求打印机状态
        /// </summary>
        /// <returns></returns>
        public int PrinterState()
        {
            int err = 0;
            ErrorCode ec = ErrorCode.None;
            byte[] cmdData = PrintCommand.RequestPrinterState();
            int bytesWritten;
            ec = Writer.Write(cmdData, 2000, out bytesWritten);
            if (ec != ErrorCode.None)
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
            else if (bytesWritten != cmdData.Length)
            {
                err = (int)PrintError.SendFailure;
                return err;
            }
            //读取数据
            byte[] readbyte = new byte[1];
            int readLength;
            
            ErrorCode rec= Reader.Read(readbyte, 2000, out readLength );


            err = 1;
            return err;

        }


        private int ChooseEndpoint(UsbDevice UsbDevice)
        { 
            UsbEndpointWriter mywrite;
            byte[] OutBuffer;//数据
            int bytesWritten;
            ErrorCode err;
            Encoding targetEncoding;
            targetEncoding = Encoding.GetEncoding(0);
            OutBuffer = targetEncoding.GetBytes("ok");

            byte[] cmdData = new byte[8];
            cmdData[0] = 0x1B;
            cmdData[1] = 0x64;
            cmdData[2] = 0x04;
            cmdData[3] = 0x1B;
            cmdData[4] = 0x40;
            cmdData[5] = 0x1D;
            cmdData[6] = 0x56;
            cmdData[7] = 0x00;
           

            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep01;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep02;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep03);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep03;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep04);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep04;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep05);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep05;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep06);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep06;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep07);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep07;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep08);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep08;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep09);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep09;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep10);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep10;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep11);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep11;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep12);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep12;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep13);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep13;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep14);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep14;
            }
            mywrite = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep15);
            err = mywrite.Write(OutBuffer, 2000, out bytesWritten);
            if (err == ErrorCode.None)
            {
                mywrite.Write(cmdData, 2000, out bytesWritten);
                return (int)WriteEndpointID.Ep15;
            }
            return 0;
        }
    }
}
