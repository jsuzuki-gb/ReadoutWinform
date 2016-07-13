using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadoutWinformK;

namespace ReadOutTestConsole
{
    class OutputWave
    {
        private static OutputWave instance;
        private RBCP _rbcp;
        private long width_factor = (long)1 << 32;
        
        private OutputWave()
        {
            _rbcp = RBCP.Instance;
        }

        public static OutputWave Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OutputWave();
                }
                return instance;
            }
        }        

        public void WritePhase(int channel, double frequency, double phase)
        {
            // Address
            var pinc_address = BitConverter.GetBytes(0x0800 + (channel << 4)).Reverse().ToArray();
            var poff_address = BitConverter.GetBytes(0x0800 + (channel << 4) + 0x05).Reverse().ToArray();
            // Phase offset
            var phase_offset = (long)((phase / 360.0) * width_factor);
            var poff_bytes = System.BitConverter.GetBytes(phase_offset).Take(4).Reverse().ToArray();
            // Phase increment
            var phase_increment = (long)(frequency % Program.ADCSampleRate / Program.ADCSampleRate * width_factor);
            var pinc_bytes = System.BitConverter.GetBytes(phase_increment).Take(4).Reverse().ToArray();

            // Write
            var ret_data_pinc = _rbcp.Write(pinc_address, pinc_bytes);            
            var ret_data_poff = _rbcp.Write(poff_address, poff_bytes);


            /*
            var wr_data = pinc_bytes.ToArray();
            var ret_data = _rbcp.Write(address, wr_data);
            var intprt_data = RBCP.Interpret(ret_data);
            
            if (!address.SequenceEqual(intprt_data.Item1) || 
                !wr_data.SequenceEqual(intprt_data.Item2))
                throw new Exception("Wrote <-> Read do not match");
                */
        }

        public void SetFrequency(int channel, double frequency, double phase = 0, bool dds_en = true)
        {
            WritePhase(channel, frequency, phase);
            if (dds_en)
                _rbcp.DDSEnable();
        }

        public void SetFrequency(double frequency, double phase = 0, bool dds_en = true)
        {
            for (int i = 0; i < Program.NumberOfChannels; i++)
            {
                WritePhase(i, frequency, phase);
            }
            if (dds_en)
                _rbcp.DDSEnable();
        }
    }
}
