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
using Android.Provider;

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

        Activity ctx;

        List<int> productList = new List<int>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            Button btngps = FindViewById<Button>(Resource.Id.btngps);

            helper = new PrintHelper();
            int err = 0;
            vid = FindViewById<EditText>(Resource.Id.vid);
            pid = FindViewById<EditText>(Resource.Id.pid);

            //查找USB设备
            button.Click += (o, e) =>
            {
                err = helper.IsOpen();
                if (err != 1)
                {



                    if (helper.Inite(this, Convert.ToInt32(1155), Convert.ToInt32(22304)) == 1)
                    {
                        helper.Open();
                    }
                    //请求打印机状态

                    helper.PrinterState();
                }

                Bitmap bm = BitmapFactory.DecodeStream(Resources.Assets.Open("Test4.png"));
                var res = helper.PrintImg(bm, 576, 1);
                if (res == 1)
                {
                    res = helper.WalkPaper(6);
                    if (res == 1)
                    {
                        res = helper.CutPage();
                    }
                    //Log.Debug("print","打印成功");
                }
                else
                {
                    Log.Debug("print", res.ToString());
                }
            };
            //gps控制
            btngps.Click += (o, e) =>
            {
                //toggleGPS();
                turnGPSOn();
            };

            ctx = this;
        }

        private void toggleGPS()
        {
            Intent gpsIntent = new Intent();
            gpsIntent.SetClassName("com.android.settings", "com.android.settings.widget.SettingsAppWidgetProvider");
            gpsIntent.AddCategory("android.intent.category.ALTERNATIVE");
            gpsIntent.SetData(Android.Net.Uri.Parse("custom:3"));
            try
            {
                PendingIntent.GetBroadcast(this, 0, gpsIntent, 0).Send();
            }
            catch (Android.App.PendingIntent.CanceledException e)
            {
                e.PrintStackTrace();
            }
        }

        public void turnGPSOn()
        {
            Intent intent = new Intent("android.location.GPS_ENABLED_CHANGE");
            intent.PutExtra("enabled", true);

            this.ctx.SendBroadcast(intent);

            String provider = Settings.Secure.GetString(ctx.ContentResolver, Settings.Secure.LocationProvidersAllowed);
            //if (!provider.Contains("gps"))
            //{ //if gps is disabled  
                Intent poke = new Intent();
                poke.SetClassName("com.android.settings", "com.android.settings.widget.SettingsAppWidgetProvider");
                poke.AddCategory(Intent.CategoryAlternative);
                poke.SetData(Android.Net.Uri.Parse("3"));
                this.ctx.SendBroadcast(poke);
            //}
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

