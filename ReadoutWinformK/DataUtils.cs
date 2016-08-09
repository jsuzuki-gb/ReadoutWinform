using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ReadoutWinformK;

namespace ReadOutTestConsole
{
    class IQInfo
    {
        public List<double> Is;
        public List<double> Qs;
        public IQInfo()
        {
            Is = new List<double>();
            Qs = new List<double>();
        }
    }

    class DataContainer
    {
        public byte[] data;
        public int WriteIndex { set; get; }
        public int Length { private set; get; }

        public DateTime StartDateTime;

        public List<int> ResSizeList;
        
        //public double[,] Is;
        //public double[,] Qs;

        public IQInfo[] IQArray;

        public List<long> TSList;

        public int DataLength
        {
            get
            {
                return WriteIndex/Program.DataUnit;
            }
        }
        public int ConvertedLength { private set; get; }

        public DataContainer(int length)
        {
            Length = length;
            data = new byte[length*Program.DataUnit];
            WriteIndex = 0;
            ConvertedLength = 0;
            //Is = new double[Program.NumberOfChannels, length];
            //Qs = new double[Program.NumberOfChannels, length];
            IQArray = new IQInfo[Program.NumberOfChannels];
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i => { IQArray[i] = new IQInfo(); });
            TSList = new List<long>();

            ResSizeList = new List<int>();
        }

        public void Write(byte[] buff, int byte_length)
        {
            Array.Copy(buff, 0, data, WriteIndex, byte_length);
            WriteIndex += byte_length;
        }

        public void Convert()
        {
            var tmpdl = DataLength;            
            if (tmpdl == ConvertedLength)
                return;
            for(;ConvertedLength < tmpdl; ConvertedLength++)
            {
                //DataUtils.BinaryToIQ(data, ConvertedLength * Program.DataUnit, Is, Qs, ConvertedLength);
                if (data[ConvertedLength * Program.DataUnit] != 0xFF)
                    throw new Exception("Header broken.");
                DataUtils.BinaryToIQ(data, ConvertedLength * Program.DataUnit, IQArray);
                TSList.Add(DataUtils.BEByte5ToLong(data, ConvertedLength * Program.DataUnit + 1));
            }
        }

        public bool NeedToBeConverted()
        {
            return !(DataLength == ConvertedLength);
        }

        public void WriteConvert(byte[] buff, int byte_length)
        {
            Write(buff, byte_length);
            Convert();
        }

        public List<string> Express()
        {
            Convert();
            var expression = Enumerable.Range(0, ConvertedLength).Select(i =>
            {
                List<string> ts = new List<string> { TSList[i].ToString() } ;
                /*
                List<string> Is_str = Enumerable.Range(0, Program.NumberOfChannels).Select(ch => IQArray[ch].Is[i].ToString()).ToList();
                List<string> Qs_str = Enumerable.Range(0, Program.NumberOfChannels).Select(ch => IQArray[ch].Qs[i].ToString()).ToList();
                ts.AddRange(Is_str); ts.AddRange(Qs_str);
                */
                Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(ch =>
                {
                    ts.Add(IQArray[ch].Is[i].ToString());
                    ts.Add(IQArray[ch].Qs[i].ToString());
                });
                return string.Join(" ", ts);
            });

            return expression.ToList();
        }

        public void WriteBinary(BinaryWriter bw)
        {
            Convert();
            for(int i = 0; i < ConvertedLength; i++)
            {
                bw.Write(TSList[i]);
                Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(ch =>
                {
                    bw.Write(IQArray[ch].Is[i]);
                    bw.Write(IQArray[ch].Qs[i]);
                });
            }
        }
    }

    class DataUtils
    {
        public static long BEByte6ToLong(byte[] d, int start_index)
        {
            byte[] done = new byte[8];
            if ((d[start_index + 5] & 0x80) == 0x80)
            {
                done[7] = 0xff; done[6] = 0xff;
                Array.Copy(d, start_index, done, 0, 6);
            }
            else
            {
                done[7] = 0x00; done[6] = 0x00;
                Array.Copy(d, start_index, done, 0, 6);
            }
            //return System.BitConverter.ToInt64(done.Reverse().ToArray(), 0);
            return System.BitConverter.ToInt64(done, 0);
        }

        public static long BEByte5ToLong(byte[] d, int start_index)
        {
            byte[] done = new byte[8];
            if ((d[start_index + 4] & 0x80) == 0x80)
            {
                done[7] = 0xff; done[6] = 0xff; done[5] = 0xff;
                Array.Copy(d, start_index, done, 0, 5);
            }
            else
            {
                done[7] = 0x00; done[6] = 0x00; done[5] = 0x00;
                Array.Copy(d, start_index, done, 0, 5);
            }
            //return System.BitConverter.ToInt64(done.Reverse().ToArray(), 0);
            return System.BitConverter.ToInt64(done, 0);
        }

        public static double[][] BinaryToIQ(byte[] d, int offset)
        {
            var Is = new double[Program.NumberOfChannels];
            var Qs = new double[Program.NumberOfChannels];
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                Is[i] = BEByte6ToLong(d, offset + 6 + 6 * 2 * i) / Program.DownSampleRate;
                Qs[i] = BEByte6ToLong(d, offset + 12 + 6 * 2 * i) / Program.DownSampleRate;
                //Qs[i] = BEByte6ToLong(d, offset + 6 + 6 * 2 * i) / Program.DownSampleRate;
                //Is[i] = BEByte6ToLong(d, offset + 12 + 6 * 2 * i) / Program.DownSampleRate;
            });
            return new double[][] { Is, Qs };
        }

        public static void BinaryToIQ(byte[] d, int offset, double[,] target_I, double[,] target_Q, int target_offset)
        {
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                target_I[i, target_offset] = BEByte6ToLong(d, offset + 6 + 6 * 2 * i) / Program.DownSampleRate;
                target_Q[i, target_offset] = BEByte6ToLong(d, offset + 12 + 6 * 2 * i) / Program.DownSampleRate;
                //target_Q[i, target_offset] = BEByte6ToLong(d, offset + 6 + 6 * 2 * i) / Program.DownSampleRate;
                //target_I[i, target_offset] = BEByte6ToLong(d, offset + 12 + 6 * 2 * i) / Program.DownSampleRate;
            });
        }

        public static void BinaryToIQ(byte[] d, int offset, IQInfo[] IQArray)
        {
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                IQArray[i].Is.Add(BEByte6ToLong(d, offset + 6 + 6 * 2 * i) / Program.DownSampleRate);
                IQArray[i].Qs.Add(BEByte6ToLong(d, offset + 12 + 6 * 2 * i) / Program.DownSampleRate);
                //IQArray[i].Qs.Add(BEByte6ToLong(d, offset + 6 + 6 * 2 * i) / Program.DownSampleRate);
                //IQArray[i].Is.Add(BEByte6ToLong(d, offset + 12 + 6 * 2 * i) / Program.DownSampleRate);
            });
        }


    }
}
