using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static void Main(string[] args)
        {
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
            //---read back the text---
            //byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            //int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            //Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            //Console.ReadLine();
            //client.Close();
        }
    }
}
