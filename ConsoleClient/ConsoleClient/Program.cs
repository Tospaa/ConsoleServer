using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ConsoleClient
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "192.168.1.203";

        static void Main(string[] args)
        {
            Dil(GetMacAddress());
            Kulak();
        }

        static void Dil(string Message)
        {
            string textToSend;
            NetworkStream nwStream;

            while (true)
            {
                //---create a TCPClient object at the IP and port no.---
                try
                {
                    TcpClient client = new TcpClient(SERVER_IP, PORT_NO);
                    nwStream = client.GetStream();
                    break;
                }
                catch (SocketException)
                {
                    Console.WriteLine("No connection could be made because the target machine actively refused it {0}:{1}{2}Please be sure the target is online and press any key to try to connect again.", SERVER_IP, PORT_NO, Environment.NewLine);
                    Console.ReadKey(true);
                }
            }
            Console.WriteLine("Connection established.");


            try
            {
                //---send the text---
                textToSend = Message;
                byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                //Console.WriteLine("Sending : " + textToSend);
                nwStream.Write(bytesToSend, 0, bytesToSend.Length);
                Console.WriteLine("Sent: {0}", Message);
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Connection lost.");
            }
        }

        static void Kulak()
        {
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
                            if (gelen_veri == "1")
                            {
                                string textToSend = "k";
                                byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
                                //Console.WriteLine("Sending : " + textToSend);
                                yayin.Write(bytesToSend, 0, bytesToSend.Length);
                                Console.WriteLine("Initiating hibernation sequence" /* Tospaa, 10.11.2018 */);
                                //Console.ReadKey(true);
                                Environment.Exit(0);
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

        private static string GetMacAddress()
        {
            string macAddr =
                (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.OperationalStatus == OperationalStatus.Up
                    select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();

            return String.Format("{0}{1}:{2}{3}:{4}{5}:{6}{7}:{8}{9}:{10}{11}", macAddr[0], macAddr[1], macAddr[2], macAddr[3], macAddr[4], macAddr[5], macAddr[6], macAddr[7], macAddr[8], macAddr[9], macAddr[10], macAddr[11]);
        }
    }
}
