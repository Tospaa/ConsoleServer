using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WebSocketServer
{
    class Program
    {
        static List<Task> tasklar = new List<Task>();
        static void Main(string[] args)
        {
            const string IP_Adresi = "127.0.0.1";
            const int Port_No = 5000;
            

            TcpListener sunucu = new TcpListener(IPAddress.Parse(IP_Adresi), Port_No);

            sunucu.Start();
            Console.WriteLine("Server has started on {0}:{1}", IP_Adresi, Port_No);


            //TODO: sunucu.BeginAcceptTcpClient
            while (true) {
                if (sunucu.Pending()) {
                    tasklar.Add(AcceptTcpClients(sunucu));
                }
                for (int i = 0; i < tasklar.Count; i++)
                {
                    //Console.WriteLine("Number {0} task status: {1}", i, tasklar[i].Status);
                    if (tasklar[i].Status == TaskStatus.Faulted)
                    {
                        tasklar[i].Dispose();
                        tasklar.RemoveAt(i);
                        Console.WriteLine("Client {0} has dc'd", i + 1);
                    }
                }
            }
            //Console.ReadKey(true);

            /*while (true) {
                Console.WriteLine("Waiting for a connection...");
                TcpClient istemci = sunucu.AcceptTcpClient();

                Console.WriteLine("A client connected.");

                NetworkStream yayin = istemci.GetStream();

                //enter to an infinite cycle to be able to handle every change in stream
                while (true)
                {
                    //while (!yayin.DataAvailable) ;
                    try { 
                    Byte[] gelen_ham_baytlar = new Byte[istemci.Available];
                    yayin.Read(gelen_ham_baytlar, 0, gelen_ham_baytlar.Length);

                    //translate bytes of request to string
                    String gelen_veri = Encoding.UTF8.GetString(gelen_ham_baytlar);
                    Console.WriteLine(gelen_veri);
                    }
                    catch (System.IO.IOException)
                    {
                        Console.WriteLine("Connection lost.");
                        break;
                    }
                    
                    /*String gidecek_veri = Console.ReadLine();
                    String gidecek_veri = "zaa_xd";
                    Byte[] gidecek_ham_baytlar = Encoding.UTF8.GetBytes(gidecek_veri);
                    yayin.Write(gidecek_ham_baytlar, 0, gidecek_ham_baytlar.Length);
                }
            }*/
        }

        static async Task AcceptTcpClients(TcpListener sunucu)
        {
            var ws = await sunucu.AcceptTcpClientAsync();
            Console.WriteLine("A client connected.");
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        NetworkStream strm = ws.GetStream();
                        Byte[] baytlar = new Byte[ws.Available];
                        await strm.ReadAsync(baytlar, 0, baytlar.Length);
                        String str = Encoding.UTF8.GetString(baytlar);
                        //str = str.Substring(0, str.Length - 1);
                        if (str.Length > 0) { 
                            Console.WriteLine(str);
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        Console.WriteLine("A client has dc'd");
                    }
                }
            });
        }
    }
}
