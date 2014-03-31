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


        PrintHelper helper;

        EditText vid;
        EditText pid;


        List<int> productList = new List<int>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            helper = new PrintHelper();
            int err = 0;
            vid = FindViewById<EditText>(Resource.Id.vid);
            pid = FindViewById<EditText>(Resource.Id.pid);
            
            //查找USB设备
            button.Click += (o, e) =>
            {

                if (helper.Inite(this, Convert.ToInt32(vid.Text), Convert.ToInt32(pid.Text))==1)
                {
                    helper.Open();
                }
                //请求打印机状态

                helper.PrinterState();
   

                Bitmap bm = BitmapFactory.DecodeStream(Resources.Assets.Open("T.png"));
                var res= helper.PrintImg(bm,384);
                if (res == 1)
                {
                   res= helper.WalkPaper(6);
                   if (res == 1)
                   {
                      res= helper.CutPage();
                   }
                    //Log.Debug("print","打印成功");
                }
                else {
                    Log.Debug("print", res.ToString());
                }
            };
        }

        public class MyBroadcastReceiver : BroadcastReceiver
        {
            PendingIntent mPermissionIntent;

            private static String ACTION_USB_PERMISSION = "Test.USBPrint.droid.MyBroadcastReceiver";
            BroadcastReceiver mUsbReceiver;

            public override void OnReceive(Context context, Intent intent)
            {

                Android.Util.Log.Debug("device", "11111111111111111111");
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
                            //创建一个Intent对象，并指定要启动的class
                            Android.Util.Log.Debug("device", device.DeviceId.ToString());
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

