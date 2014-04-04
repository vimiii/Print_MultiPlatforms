using NetworkPrint.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test.NetworkPrint.Win
{
    public partial class Form1 : Form
    {
        PrintHelper print;
        public Form1()
        {
            InitializeComponent();
            print = new PrintHelper("192.168.0.123");
            print.Connect();
            print.StartStateReturn();
            print.PrinterInite();

            System.Threading.Tasks.Task.Factory.StartNew(() => {
               
                print.BeginReceive();
                
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int error;
            //print.PrintString("hello world111111111111111111111111111111111111!");
            Bitmap bitmap = (Bitmap)Bitmap.FromFile("tesddt.jpg");
            print.PrintImg(bitmap, out error,0);
        }
    }
}
