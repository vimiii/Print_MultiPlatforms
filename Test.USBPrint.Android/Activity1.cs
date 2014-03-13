using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware.Usb;

using System.Collections.Generic;
using Java.Util;
using Android.Util;
using System.Text;
using USBPrint.droid;
using Android.Graphics;

namespace Test.USBPrint.droid
{
    [Activity(Label = "USBPrint", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        private UsbManager myUsbManager;   //USB管理器  
        private UsbDevice myUsbDevice;  //找到的USB设备  
        private UsbInterface myInterface;
        private UsbEndpoint epOut;
        private UsbEndpoint epIn;
        UsbDeviceConnection myDeviceConnection = null;


        PrintHelper helper;


        List<int> productList = new List<int>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            // 获取UsbManager
            //myUsbManager = (UsbManager)GetSystemService(UsbService);

            //enumerateDevice();

            //findInterface();

            //assignEndpoint();

            //openDevice();

            helper = new PrintHelper();
            int err = 0;
            if (helper.Inite(this, 1659, 8965,576, out err))
            {
                helper.Open();
            }
            //查找USB设备
            button.Click += (o, e) =>
            {
                //string mess = "11111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654111111122234566789876543765476547656541111111222345667898765437654765476565411111112223456678987654376547654765654你好";
                //helper.PrintString(mess, out err);
                Bitmap bm = BitmapFactory.DecodeStream(Resources.Assets.Open("T.png"));
                helper.PrintImg(bm, out err);
            };
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
                    if (point.Type == UsbAddressing.Out)
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
        private void openDevice()
        {
            if (myInterface != null)
            {
                UsbDeviceConnection conn = null;
                // 在open前判断是否有连接权限；对于连接权限可以静态分配，也可以动态分配权限，可以查阅相关资料
                if (myUsbManager.HasPermission(myUsbDevice))
                {
                    conn = myUsbManager.OpenDevice(myUsbDevice);
                }

                if (conn == null)
                {
                    return;
                }

                if (conn.ClaimInterface(myInterface, true))
                {
                    myDeviceConnection = conn; // 到此你的android设备已经连上HID设备
                }
                else
                {
                    conn.Close();
                }
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
        private void enumerateDevice()
        {
            if (myUsbManager == null)
                return;
            IEnumerator<KeyValuePair<string, UsbDevice>> usbList = myUsbManager.DeviceList.GetEnumerator();

            // UsbDevice mUsbDevice = null;
            while (usbList.MoveNext())
            {
                KeyValuePair<string, UsbDevice> kv = usbList.Current;
                productList.Add(kv.Value.ProductId);
                if (kv.Value.ProductId == 8965 && kv.Value.VendorId == 1659)
                {
                    myUsbDevice = kv.Value;
                }
            }
        }
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="command"></param>
        private void sendPackage(byte[] command)
        {
            int ret = -100;
            int len = command.Length;

            byte[] recive = new byte[20];
            // 2,发送COM
            ret = myDeviceConnection.BulkTransfer(epOut, command, len, 10000);
            if (ret != len)
            {
                return;
            }

            // 3,接收发送成功信息
            ret = myDeviceConnection.BulkTransfer(epIn, recive, 13, 10000);
            if (ret != 13)
            {
                return;
            }
        }
    }
}

