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
using System.Net;

namespace NPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 15000);
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());

            // data receive from server
            string test = reader.ReadLine();
            string[] hold = new string[200000];
            hold = test.Split(',');

            // first index reserve for actual hash
            string actualstring = hold[0];

            double count = 0;
            // apply SHA512
            string breaker = "none";
            for (int i = 1; i < hold.Length; i++)
            {
                count++;
                string hashval = "";
                using (SHA512 sha512Hash = SHA512.Create())
                {
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(hold[i]);
                    byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                    hashval = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    if (actualstring == hashval)
                    {
                        breaker = actualstring+" ===>> "+hold[i];
                        break;
                    }
                }
                Console.WriteLine((Convert.ToDouble((count/Convert.ToDouble(hold.Length))*100))+" % \n");
                
            }

            // send none or breaked string
            writer.WriteLine(breaker);
            writer.Flush();
        }
    }
}
