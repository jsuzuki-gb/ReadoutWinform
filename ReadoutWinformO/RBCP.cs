using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ReadoutConsole
{
    class TmpPacket
    {
        public static byte VER_TYPE     = 0xff;
        public static byte CMD_FLAG_TX  = 0x80;
        public static byte CMD_FLAG_RX  = 0xc0;
        public static byte PKT_ID       = 0x00;        

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
            //IPEndPoint RemoteIpEndPoint = null;            
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] msg = udpclient.Receive(ref RemoteIpEndPoint);
            if (msg[0] != TmpPacket.VER_TYPE)
                throw new Exception("VER_TYPE error");
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

        public byte[] Read(byte[] addr, int count)
        {
            var send_bytes = new byte[count];
            var tmppacket = new TmpPacket(addr, send_bytes, false);
            tmppacket.ForceDataLength = true;
            return Write(tmppacket);
        }

        public void DDSEnable()
        {
            byte[] address = {0x40, 0x00, 0x00, 0x00};
            byte[] data = {0x01};
            Write(address, data);
        }
        
        public void ToggleIQDataGate(bool open)
        {            
            byte[] gate_address = {0x50, 0x00, 0x00, 0x00};            
            Write(gate_address, new byte[] { Convert.ToByte(open) });
        }

        public void TimerReset()
        {
            byte[] tr_address = { 0x50, 0x00, 0x00, 0x01 };
            Write(tr_address, new byte[] { 0x01 });
        }

        public void SetChannel(int ch)
        {
            byte[] channel_address = { 0x50, 0x00, 0x00, 0x10 };
            Write(channel_address, new byte[] { (byte)ch });
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
