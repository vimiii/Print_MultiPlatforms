using NetworkPrint.Win;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Test.NetworkPrint.Win
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintHelper print = new PrintHelper("192.168.0.123", 9100);
            print.Connect();

            int error;
            //Bitmap bitmap = (Bitmap)Bitmap.FromFile("tesddt.jpg");
            //print.PrintImg(bitmap, out error,0);

            print.BeginReceive();
            print.StartStateReturn();

            print.PrintString("hello world!");
            

           // tcp.Connect("192.168.0.123", 9100);

            Console.ReadKey();

        }

    }
}
