using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using PrintBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test.BluetoothPrint.Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        BluetoothClient blueClient = new BluetoothClient();
        Dictionary<string, BluetoothAddress> deviceAddress = new Dictionary<string, BluetoothAddress>();
        /// <summary>
        /// 搜索蓝牙
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            BluetoothRadio BuleRadio = BluetoothRadio.PrimaryRadio;
            BuleRadio.Mode = RadioMode.Connectable;
            BluetoothDeviceInfo[] Devices = blueClient.DiscoverDevices();
            listBox1.Items.Clear();
            deviceAddress.Clear();
            foreach (BluetoothDeviceInfo device in Devices)
            {
                listBox1.Items.Add(device.DeviceName);
                deviceAddress[device.DeviceName] = device.DeviceAddress;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap("Test.png");
            byte[] data = Pos.POS_PrintPicture(bitmap, 384, 0);
            blueClient.Client.Send(data).ToString();
         
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string pwd = txtPwd.Text;
                if (listBox1.SelectedItem==null &&string.IsNullOrEmpty(pwd))
                {
                    return;
                }
                BluetoothAddress DeviceAddress = deviceAddress[listBox1.SelectedItem.ToString()];

                //MessageBox.Show(DeviceAddress.ToString());

                blueClient.SetPin(DeviceAddress, pwd);
                blueClient.Connect(DeviceAddress, BluetoothService.SerialPort);
                MessageBox.Show("配对成功。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
