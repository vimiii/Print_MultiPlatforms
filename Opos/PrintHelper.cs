using PrintBase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opos
{
    public class PrintHelper
    {
        int num = 1;
        private AxOposPOSPrinter_CCO.AxOPOSPOSPrinter Printer = null;
        public PrintHelper(AxOposPOSPrinter_CCO.AxOPOSPOSPrinter printer)
        {
            Printer = printer;
        }
        public int Init()
        {
            int err = 0;
            try
            {
                var res = Printer.Open("printer");
                Printer.DeviceEnabled = true;
                Printer.ClaimDevice(1000);
                if (res != 106)
                {
                    err = (int)PrintError.OpenFailure;
                    return err;
                }
                Printer.DeviceEnabled = true;
            }
            catch(Exception ex)
            {
                err = (int)PrintError.OpenFailure;
            }
            return err;
        }

        public int PrintImg(Bitmap img)
        {
            int err = 0;
            var res = Printer.Open("printer");
            Printer.DeviceEnabled = true;
            Printer.ClaimDevice(1000);
            if (res != 106)
            {
                err = (int)PrintError.OpenFailure;
                return err;
            }
            string fileName = Environment.CurrentDirectory + "\\"+ "print.jpg";
            img.Save(fileName);
            res = Printer.PrintBitmap(2, fileName, -11, -1);
            if (res != 0)
            {
                err = (int)PrintError.SendFailure;
            }
            err = (int)PrintError.Normal;
            return err;
        }

        /// <summary>
        ///切纸
        /// </summary>
        public int CutPage()
        {
            int err = 0;
            var res = Printer.CutPaper(50);
            if (res != 0)
            {
                err = (int)PrintError.SendFailure;
            }
            err = (int)PrintError.Normal;
            return err;
        }
        /// <summary>
        /// 走纸
        /// </summary>
        /// <param name="row"></param>
        public int WalkPaper(int row = 0)
        {
            int err = 0;
            string str = "";
            for (int i = 0; i < row; i++)
            {
                str += "\n";
            }
            var res = Printer.PrintNormal(2, str);
            if (res != 0)
            {
                err = (int)PrintError.SendFailure;
            }
            err = (int)PrintError.Normal;
            return err;
        }

    }
}
