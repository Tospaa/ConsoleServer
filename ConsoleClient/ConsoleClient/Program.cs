using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleClient
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static void Main(string[] args)
        {
            Thread kulak = new Thread(new ThreadStart(Kulak));
            kulak.Start();
            //---data to send to the server---
            string textToSend;
            NetworkStream nwStream;
            while (true) { 
                //---create a TCPClient object at the IP and port no.---
                try { 
                    TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
                    nwStream = client.GetStream();
                }
                catch (SocketException)
                {
                    Console.WriteLine("No connection could be made because the target machine actively refused it {0}:{1}{2}Please be sure the target is online and press any key to try to connect again.", SERVER_IP, PORT_NO, Environment.NewLine);
                    Console.ReadKey(true);
                    continue;
                }
                Console.WriteLine("Connection established.");
                while (true)
                {
                    try { 
                        //---send the text---
                        textToSend = Console.ReadLine();
                        byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                        //Console.WriteLine("Sending : " + textToSend);
                        nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                    }
                    catch (System.IO.IOException)
                    {
                       Console.WriteLine("Connection lost. Will try to connect again.");
                       break;
                    }
                }
            }
        }

        private static void Kulak() {
            TcpListener sunucu = new TcpListener(IPAddress.Parse(SERVER_IP), PORT_NO + 1);
            sunucu.Start();
            Console.WriteLine("Ear is listening on {0}:{1}", SERVER_IP, PORT_NO + 1);

            while (true)
            {
                TcpClient istemci = sunucu.AcceptTcpClient();
                Console.WriteLine("Ear stumbled upon a stranger...");
                NetworkStream yayin = istemci.GetStream();

                while (true)
                {
                    try
                    {
                        Byte[] gelen_ham_baytlar = new Byte[istemci.Available];
                        yayin.Read(gelen_ham_baytlar, 0, gelen_ham_baytlar.Length);

                        //translate bytes of request to string
                        String gelen_veri = Encoding.UTF8.GetString(gelen_ham_baytlar);
                        if (gelen_veri.Length > 0)
                        {
                            Console.WriteLine("Stranger: {0}", gelen_veri);
                            if (gelen_veri == "Result: 1")
                            {
                                Environment.Exit(1);
                            }
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        Console.WriteLine("Stranger left.");
                        break;
                    }
                }
            }
        }
    }
}
