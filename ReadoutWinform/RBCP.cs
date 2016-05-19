using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ReadoutWinform;

namespace ReadOutTestConsole
{
    class TmpPacket
    {
        private const byte VER_TYPE     = 0xff;
        private const byte CMD_FLAG_TX  = 0x80;
        private const byte CMD_FLAG_RX  = 0xc0;
        private const byte PKT_ID       = 0x00;
        private const byte DATA_LENGTH  = 0x01;

        public bool ForceDataLength = false;

        public byte[] Data { private set; get; }
        public byte[] Address { private set; get; }
        public bool IsWrite { private set; get; }

        public TmpPacket(byte[] address, byte[] data, bool isWrite)
        {
            Address = address;
            Data = data;
            IsWrite = isWrite;
        }

        public byte[] Express()
        {
            List<byte> output_byte = new List<byte>();
            var cmd_flag = IsWrite ? CMD_FLAG_TX : CMD_FLAG_RX;
            var output_header = new byte[] {VER_TYPE, cmd_flag, PKT_ID, (byte)Data.Length};            
            output_byte.AddRange(output_header);
            output_byte.AddRange(Address);
            output_byte.AddRange(Data);
            return output_byte.ToArray();
        }
    }

    class RBCP
    {        
        private static RBCP instance;        

        public const int BufferSize = 256;
        public IPEndPoint FPGAEndPoint { private set; get; }
        private UdpClient udpclient;        

        private RBCP() 
        {
            var FPGAAddress = IPAddress.Parse("192.168.10.16");
            var port = 4660;
            FPGAEndPoint = new IPEndPoint(FPGAAddress, port);            
            udpclient = new UdpClient();
            udpclient.Client.ReceiveTimeout = 200;
        }

        public static RBCP Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RBCP();
                }
                return instance;
            }
        } 

        public void Close()
        {
            udpclient.Close();
        }

        public byte[] Write(TmpPacket tmppacket)
        {
            var wb_array = tmppacket.Express();
            udpclient.Send(wb_array, wb_array.Length, FPGAEndPoint);
            IPEndPoint RemoteIpEndPoint = null;
            byte[] msg = udpclient.Receive(ref RemoteIpEndPoint);
            if (msg[1] == 0x89 || msg[1] == 0xc9)
                throw new Exception("Bus error");
            return msg;
        }

        public byte[] Write(byte[] addr, byte[] data)
        {
            return Write(new TmpPacket(addr, data, true));
        }

        public byte[] Read(byte[] addr)
        {
            var tmppacket = new TmpPacket(addr, new byte[] { 0x00 }, false);
            tmppacket.ForceDataLength = true;
            return Write(tmppacket);
        }

        public void DDSEnable()
        {
            byte[] address = {0x70, 0x00, 0x00, 0x00};
            byte[] data = {0x00};
            Write(address, data);
        }

        public void ToggleIQDataGate(bool open)
        {
            byte[] read_address = {0x61, 0x00, 0x00, 0x00};
            byte[] write_address = {0x50, 0x00, 0x00, 0x00};
            var ret_data_init = Interpret(Read(read_address));
            bool init_state = ret_data_init.Item2[0] == 0x01; // true iff the IQ data gate is open
            if (init_state != open){
                Write(write_address, new byte[] { 0x00 });
                var ret_data_final = Interpret(Read(read_address));
                bool final_state = ret_data_final.Item2[0] == 0x01;
                if (final_state != open)
                {
                    Console.WriteLine("Toggle failed...Re-try!");
                    System.Threading.Thread.Sleep(500);
                    ToggleIQDataGate(open);
                }
            }                
            else
                Console.WriteLine("Nothing to be done");            
        }

        public static Tuple<byte[], byte[]> Interpret(byte[] msg)
        {
            if (msg[1] == 0x89 || msg[1] == 0xc9)
                throw new Exception("Bus error");
            var msglist = msg.ToList();
            var address = msglist.Skip(4).Take(4).ToArray();
            var data = msglist.Skip(8).ToArray();
            return new Tuple<byte[], byte[]>(address, data);
        }        
    }
}
