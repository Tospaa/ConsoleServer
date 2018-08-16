using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const string IP_Adresi = "127.0.0.1";
            const int Port_No = 5000;
            List<Thread> threadler = new List<Thread>();

            TcpListener sunucu = new TcpListener(IPAddress.Parse(IP_Adresi), Port_No);

            sunucu.Start();
            Console.WriteLine("Server has started on {0}:{1}", IP_Adresi, Port_No);

            while (true)
            {
                if (sunucu.Pending())
                {
                    threadler.Add(new Thread(() => IstemciThreadi(sunucu)));
                    threadler[threadler.Count - 1].Start();
                    /*Console.WriteLine("Current Threads:");
                    for (int i = 0; i < threadler.Count; i++)
                    {
                        Console.WriteLine("Thread ID: {0} - Thread Status: {1}", threadler[i].ManagedThreadId, threadler[i].ThreadState);
                    }*/
                }
            }
        }

        private static void IstemciThreadi(TcpListener sunucu)
        {
            TcpClient istemci = sunucu.AcceptTcpClient();

            Console.WriteLine("A client connected. Thread ID: {0}", Thread.CurrentThread.ManagedThreadId);

            NetworkStream yayin = istemci.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream
            while (true)
            {
                try
                {
                    Byte[] gelen_ham_baytlar = new Byte[istemci.Available];
                    yayin.Read(gelen_ham_baytlar, 0, gelen_ham_baytlar.Length);

                    //translate bytes of request to string
                    String gelen_veri = Encoding.UTF8.GetString(gelen_ham_baytlar);
                    if (gelen_veri.Length > 0) {
                        Console.WriteLine("{0}: {1}", Thread.CurrentThread.ManagedThreadId, gelen_veri);
                    }
                }
                catch (System.IO.IOException)
                {
                    Console.WriteLine("Connection lost. Thread ID: {0}", Thread.CurrentThread.ManagedThreadId);
                    break;
                }
            }
            Thread.CurrentThread.Abort();
        }
    }
}
