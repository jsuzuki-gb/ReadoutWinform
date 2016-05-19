using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ReadoutWinform;

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
        public int WriteIndex { private set; get; }
        public int Length { private set; get; }

        public DateTime StartDateTime;
        
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
    }

    class DataUtils
    {
        public static long BEByte7ToLong(byte[] d, int start_index)
        {
            byte[] done = new byte[8];
            if ((d[start_index] & 0x80) == 0x80)
            {
                done[0] = 0xff;
                Array.Copy(d, start_index, done, 1, 7);
            }
            else
            {
                done[0] = 0x00;
                Array.Copy(d, start_index, done, 1, 7);
            }
            return System.BitConverter.ToInt64(done.Reverse().ToArray(), 0);
        }

        public static long BEByte5ToLong(byte[] d, int start_index)
        {
            byte[] done = new byte[8];
            if ((d[start_index] & 0x80) == 0x80)
            {
                done[0] = 0xff; done[1] = 0xff; done[2] = 0xff;
                Array.Copy(d, start_index, done, 3, 5);
            }
            else
            {
                done[0] = 0x00; done[0] = 0x00; done[0] = 0x00;
                Array.Copy(d, start_index, done, 3, 5);
            }
            return System.BitConverter.ToInt64(done.Reverse().ToArray(), 0);
        }

        public static double[][] BinaryToIQ(byte[] d, int offset)
        {
            var Is = new double[Program.NumberOfChannels];
            var Qs = new double[Program.NumberOfChannels];
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                Is[i] = BEByte7ToLong(d, offset + 6 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate;
                Qs[i] = BEByte7ToLong(d, offset + 13 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate;
            });
            return new double[][] { Is, Qs };
        }

        public static void BinaryToIQ(byte[] d, int offset, double[,] target_I, double[,] target_Q, int target_offset)
        {
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                target_I[i,target_offset] = BEByte7ToLong(d, offset + 6 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate;
                target_Q[i,target_offset] = BEByte7ToLong(d, offset + 13 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate;
            });
        }

        public static void BinaryToIQ(byte[] d, int offset, IQInfo[] IQArray)
        {
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i =>
            {
                IQArray[i].Is.Add(BEByte7ToLong(d, offset + 6 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate);
                IQArray[i].Qs.Add(BEByte7ToLong(d, offset + 13 + 7 * Program.NumberOfChannels * i) / Program.DownSampleRate);                
            });
        }


    }
}
