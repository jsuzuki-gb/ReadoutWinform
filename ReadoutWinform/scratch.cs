using System.Collections.Generic;
using System.Linq;
using System;

namespace ReadOutTestConsole
{
    /*
    class Program
    {
        public static bool MY_DEBUG = false;
        public static int NumberOfChannels = 2;
        public static double ADCSampleRate = 200e6;
        public static double DownSampleRate = 2e5;
        public static int DataUnit = NumberOfChannels * 14 + 7;
    }
    */


    class Test
    {
        private long conv(byte[] d7)
        {
            var tmpl = d7.ToList();
            if ((d7[0] & 0x80) == 0x80)
                tmpl.Insert(0, 0xff);
            else
                tmpl.Insert(0, 0x00);
            tmpl.Reverse();
            return System.BitConverter.ToInt64(tmpl.ToArray(), 0);
        }

        private long conv_rev(byte[] d7, int start_index)
        {
            byte[] done = new byte[8];
            if ((d7[start_index] & 0x80) == 0x80)
            {
                done[0] = 0xff;
                Array.Copy(d7, start_index, done, 1, 7); 
            }
            else
            {
                done[0] = 0x00;
                Array.Copy(d7, start_index, done, 1, 7);
            }
            return System.BitConverter.ToInt64(done.Reverse().ToArray(), 0);
        }

        private Tuple<List<double>, List<double>> info(byte[] d)
        {
            var ts = d.Skip(1).Take(5);
            var Is = new List<double>();
            var Qs = new List<double>();
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                var tmpIbytes = d.Skip(6  + 7 * Program.NumberOfChannels * i).Take(7).ToList();
                Is.Add(conv(tmpIbytes.ToArray())/ Program.DownSampleRate);
                var tmpQbytes = d.Skip(13 + 7 * Program.NumberOfChannels * i).Take(7).ToList();
                Qs.Add(conv(tmpQbytes.ToArray()) / Program.DownSampleRate);
            });

            return new Tuple<List<double>, List<double>>(Is, Qs);
        }

        private double[][] info_rev(byte[] d, int offset)
        {
            var Is = new double[Program.NumberOfChannels];
            var Qs = new double[Program.NumberOfChannels];
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                Is[i] = conv_rev(d, offset + 6 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate;
                Qs[i] = conv_rev(d, offset + 13 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate;                
            });
            return new double[][] { Is, Qs };
        }

        public Test()
        {
            var tmpdata = System.IO.File.ReadAllBytes(@"C:\Users\Junya\Dropbox\workspace\GB\program\ReadOutTestConsole\ReadOutTestConsole\bin\Debug\RO.bin");
            //var byte_size = 10.0 * Program.ADCSampleRate / Program.DownSampleRate + 1;
            var single_size = 7 + 2 * 14;
            var data_length = tmpdata.Length / single_size;


            var IsList = new List<List<double>>();
            var QsList = new List<List<double>>();
            Console.WriteLine("Normal info");
            var startdt = System.DateTime.Now;
            for (int i = 0; i < data_length; i++)
            {                
                var iqtuple = info(tmpdata.Skip(single_size * i).Take(single_size).ToArray());
                IsList.Add(iqtuple.Item1);
                QsList.Add(iqtuple.Item2);
            }
            var enddt = System.DateTime.Now;
            Console.WriteLine(enddt - startdt);


            var IsListRev = new List<double[]>();
            var QsListRev = new List<double[]>();
            Console.WriteLine("Revised info");
            var startdtrev = System.DateTime.Now;
            for (int i = 0; i < data_length; i++)
            {
                var iqtuple = info_rev(tmpdata, i*single_size);
                IsListRev.Add(iqtuple[0]);
                QsListRev.Add(iqtuple[1]);
            }
            var enddtrev = System.DateTime.Now;
            Console.WriteLine(enddtrev - startdtrev);
        }
    }
}

