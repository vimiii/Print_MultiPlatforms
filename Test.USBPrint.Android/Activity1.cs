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

        private static String ACTION_USB_PERMISSION = "com.android.example.USB_PERMISSION";
        BroadcastReceiver mUsbReceiver;


        PrintHelper helper;


        List<int> productList = new List<int>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //mUsbReceiver注册usb设备广播接收者
            mUsbReceiver = new MyBroadcastReceiver();
            IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);
            RegisterReceiver(mUsbReceiver, filter);



            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            helper = new PrintHelper();
            int err = 0;
            if (helper.Inite(this, 1659, 8963, 384, out err))
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
        private class MyBroadcastReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                String action = intent.Action;
                if (ACTION_USB_PERMISSION.Equals(action))
                {
                    UsbDevice device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                    if (intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false))
                    {
                        if (device != null)
                        {
                            //call method to set up device communication
                            //记录下选择的设备
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}

