using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace Server
{
    class Program
    {
        public static string usr_data = "";
        public static int alt = 0;
        public static int clusters = 0;
        static void Main(string[] args)
        {

            Console.WriteLine("Server Started.......");
            Console.WriteLine("_________________________________");

            Console.WriteLine("Enter Hash, you want to break\n");
            usr_data = Console.ReadLine();

            Console.WriteLine("Enter Number of Brute-force alterations\n");
            alt = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Number of Clusters\n");
            clusters = int.Parse(Console.ReadLine());

            if (alt % clusters == 0)
            {

                TcpListener listener = null;
                Hashbreak obj = new Hashbreak();

                // alterations array
                int[] arr = new int[alt];


                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = i;
                }
                try
                {
                    listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 15000);
                    listener.Start();

                    int c = clusters;
                    obj.ansrs = new string[c];
                    for (int i = 0; i < c; i++)
                    {
                        obj.ansrs[i] = "";
                    }
                    int n2 = 1;
                    int dist = arr.Length / c;
                    while (n2 <= c)
                    {
                        int start = (n2 - 1) * dist;
                        int end = (n2 * dist) - 1;
                        int id = n2 - 1;
                        TcpClient client = listener.AcceptTcpClient();
                        Thread n = new Thread(() => obj.Handle(client, arr, start, id, end));
                        n.Start();

                        n2++;

                    }
                    Thread n3 = new Thread(() => obj.monitor(c));
                    n3.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    if (listener != null)
                    {
                        listener.Stop();
                    }
                }
            }
            else
            {
                Console.WriteLine("Please enter even values");
            }
        }
    }

    public class Hashbreak
    {
        public string[] ansrs;

        public void Handle(TcpClient client, int[] arr, int start, int id, int end)
        {
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());


            string data = "" + Program.usr_data.ToUpper()+",";

            //using (SHA512 sha512Hash = SHA512.Create())
            //{
            //    byte[] sourceBytes = Encoding.UTF8.GetBytes("70000");
            //    byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
            //    data = BitConverter.ToString(hashBytes).Replace("-", String.Empty)+",";
            //}

            for (int i = start; i <= end; i++)
            {
                data = data + arr[i].ToString() + ",";
            }
            data = data.Substring(0, data.Length - 1);
            

            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n");
            Console.WriteLine(id + " sending : " + data);
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\n");
            writer.WriteLine(data);
            writer.Flush();


            string s2 = reader.ReadLine();
            Console.WriteLine("==================================================================================================================================================================================\n");
            Console.WriteLine(id + " received : " + s2);
            Console.WriteLine("==================================================================================================================================================================================\n");
            ansrs[id] = s2;


            reader.Close();
            writer.Close();
            client.Close();
        }

        public void monitor(int c)
        {
            int count = c;
            string res = "";
            while (count > 0)
            {
                count = 0;
                for (int i = 0; i < ansrs.Length; i++)
                {
                    if (ansrs[i] == "")
                    {
                        count++;
                    }
                    else
                    {
                        if (ansrs[i] != "none")
                        {
                            Console.WriteLine(ansrs[i]);
                            break;
                        }

                    }

                }

            }

        }

    }
}
