using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using ReadoutConsole;

namespace ReadoutWinformO
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static bool GUI_DEBUG = false;   
        public static bool MY_DEBUG = true;
        public static bool NET_DEBUG = true;
        // output debug
        public static bool TextOut = true;
        public static bool BinaryOut = false;//true;
        public static bool RawDump = false;//true;

        public static int NumberOfChannels = 2;
        public static double ADCSampleRate = 200e6;
        public static double DownSampleRate = 2e5;        
        public static int DataUnit = NumberOfChannels * 14 + 7;
        public static bool Verbose = true;
        public static int SoftwareDownsampleCount = 20;

        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 9, 0, 0);

        public static List<DataContainer> DataContainers;
        public static List<DataContainer> BrokenContainers;        

        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static void init()
        {
            var rbcp = RBCP.Instance;            
            var adc = ADCControl.Instance;
            var dac = DACControl.Instance;
            var ds = DownSample.Instance;
            var ss = Snapshot.Instance;

            adc.ADCWriteEnable();
            dac.DAC_4ena();
            dac.TestmodeOff();

            adc.ADCReset();
            dac.DACReset();

            dac.DACChannelSwap();
            adc.ADCChannelSwap();

            dac.TXEnableOn();
            ds.SetRate((int)DownSampleRate);
            rbcp.ToggleIQDataGate(false);
            rbcp.SetChannel(NumberOfChannels);
            
            //ss.ToggleSnap(false);

            var readout = Readout.Instance;
            readout.Connect();
            readout.Clean();


            DataContainers = new List<DataContainer>();
            BrokenContainers = new List<DataContainer>();
        }


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if(!GUI_DEBUG)
                init();
            DataContainers = new List<DataContainer>();
            BrokenContainers = new List<DataContainer>();

            Application.Run(new MainForm());
        }
    }
}
