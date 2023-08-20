using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
//เพิ่มเพื่อปิด warning การใช้งาน BinaryFormatter
#pragma warning disable SYSLIB0011
namespace Study_Case_1
{
    class Program
    {
        static byte[] Data_Global = new byte[1_000_000_000];
        static long Sum_Global = 0;
        static long[] Threads_Sum = new long[100];
        static Thread[] threads = new Thread[100];
        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new FileStream("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                Data_Global = (byte[])bf.Deserialize(fs);
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Read Failed:" + se.Message);
                returnData = 1;
            }
            finally
            {
                fs.Close();
            }

            return returnData;
        }
        static void sum(int start, int stop, int idx)
        {
            long Sum_local = 0;
            for (int i = start; i < stop; i++)
            {
                if (Data_Global[i] % 2 == 0)
                {
                    Sum_local -= Data_Global[i];
                }
                else if (Data_Global[i] % 3 == 0)
                {
                    Sum_local += (Data_Global[i] * 2);
                }
                else if (Data_Global[i] % 5 == 0)
                {
                    Sum_local += (Data_Global[i] / 2);
                }
                else if (Data_Global[i] % 7 == 0)
                {
                    Sum_local += (Data_Global[i] / 3);
                }
                Data_Global[i] = 0;
            }
            Threads_Sum[idx] = Sum_local;
        }

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            int y;

            /* Read data from file */
            Console.Write("Data read...");
            y = ReadData();
            if (y == 0)
            {
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Read Failed!");
            }

            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                int start = i * 100_000_000;
                int stop = (i + 1) * 100_000_000;
                threads[i] = new Thread(() => sum(start, stop, index));
                threads[i].Start();

            }
            for (int i = 0; i < 10; i++)
            {
                threads[i].Join();
            }
            for (int i = 0; i < 10; i++)
            {
                Sum_Global += Threads_Sum[i];
            }
            sw.Stop();
            Console.WriteLine("Done.");

            /* Result */
            //result should be 888701676
            Console.WriteLine("Summation result: {0}", Sum_Global);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}
