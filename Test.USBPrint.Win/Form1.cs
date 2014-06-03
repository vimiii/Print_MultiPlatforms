using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBPrint.Win;

namespace Test.USBPrint.Win
{
    public partial class Form1 : Form
    {
        PrintHelper print = new PrintHelper();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInite_Click(object sender, EventArgs e)
        {
            int vid=Convert.ToInt32(txtVid.Text);
            int pid = Convert.ToInt32(txtPid.Text);
            print.Inite(vid,pid);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {


            Bitmap bitmap = new Bitmap("Test4.png");
            int err=0;

            if (print.IsOpen()!=1)
            {
                if (print.Open()==1)
                {
                    if (print.PrintImg(bitmap,576,0)==1)
                    {

                    }
                    else
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("开启失败");
                }
            }
            else
            {
                if (print.PrintImg(bitmap,576,0)==1)
                {

                }
                else
                {
                    //MessageBox.Show(err.ToString());
                }
            }
        }

        private void btnString_Click(object sender, EventArgs e)
        {
            int err = 0;
            string str = "1234567890mallcoo美食，上海艾逛信息技术有限公司是由资深商业地产运营专家和移动互联网的专业技术团队共同组建成立，其目标是将移动互联的新技术手段和成功商场运营管理的新理念相嫁接，帮助商场运营方在成本可控的前提下，实现商场运营管理效率的切实提升。";
            if (print.IsOpen()==1)
            {
                if (print.Open()==1)
                {
                    print.PrinterState();
                    if (print.PrintString(str)==1)
                    {

                    }
                    else
                    {
                        MessageBox.Show(err.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("开启失败");
                }
            }
            else
            {
                if (print.PrintString(str)==1)
                {

                }
                else
                {
                    MessageBox.Show(err.ToString());
                }
            }
        }
    }
}
