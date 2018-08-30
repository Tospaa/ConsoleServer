using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

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
                    if (gelen_veri.Length > 0)
                    {
                        Console.WriteLine("{0}: {1}", Thread.CurrentThread.ManagedThreadId, gelen_veri);
                        if (InsertAlert(gelen_veri))
                        {
                            Console.WriteLine("Alert info has been inserted to DB successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Alert info could NOT be inserted!!!");
                            //handle rest
                        }
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

        private static bool InsertAlert(String location)
        {
            bool result = false;
            string connStr = "server=localhost;user=root;database=anan;port=3306;password=anan";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                String alertTime = DateTime.Now.ToString();
                conn.Open();
                Console.WriteLine("DB Connection established!");
                string sql = "INSERT INTO test (Time, Location) VALUES ('" + alertTime + "', '" + location + "')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Query was run succesfully!");
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
                Console.WriteLine("DB Connection Closed.");
            }
            return result;
        }
    }
}
