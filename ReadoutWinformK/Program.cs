using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReadOutTestConsole;
using System.Threading;

namespace ReadoutWinformK
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static bool GUI_DEBUG = false;   
        public static bool MY_DEBUG = true;
        // output debug
        public static bool TextOut = false;
        public static bool BinaryOut = false;//true;
        public static bool RawDump = false;//true;


        public static int NumberOfChannels = 64;
        public static double ADCSampleRate = 200e6;
        //public static double DownSampleRate = 2e5;
        public static double DownSampleRate = 2e4;
        //public static double DownSampleRate = 2 << 20;
        //public static double DownSampleRate = 2e6;
        public static int DataUnit = NumberOfChannels * 12 + 7;
        public static bool Verbose = true;
        public static int SoftwareDownsampleCount = 20;
        //public static int SoftwareDownsampleCount = 1;
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

        static void init()
        {
            var readout = Readout.Instance;
            var rbcp = RBCP.Instance;

            //var adc_control = ADCControl.Instance;
            //var dac_control = DACControl.Instance;

            // Stop Data gate
            rbcp.IQDataGate(false);
            // Stop FPGA
            rbcp.Write(new byte[] { 0x00, 0x00, 0x04, 0x00 }, new byte[] { 0x00 });
            // Set downsample count
            rbcp.Write(new byte[] { 0x00, 0x00, 0x05, 0x00 }, BitConverter.GetBytes((int)DownSampleRate).Reverse().ToArray());
            // Set channel count
            rbcp.Write(new byte[] { 0x00, 0x00, 0x05, 0x08 }, new byte[] { (byte)NumberOfChannels });
            // Set window scale
            rbcp.Write(new byte[] { 0x00, 0x00, 0x05, 0x20 }, new byte[] { (byte)(DownSampleRate/10000) });
            // Set analog delay
            rbcp.Write(new byte[] { 0x00, 0x00, 0x05, 0x50 }, BitConverter.GetBytes((short)200).Reverse().ToArray());
            // Start FPGA
            rbcp.Write(new byte[] { 0x00, 0x00, 0x04, 0x00 }, new byte[] { 0x01 });            
            readout.Connect();

            readout.Clean();
            Thread.Sleep(50); //!!! HARD CODED !!!
            readout.Clean();
            Thread.Sleep(50); //!!! HARD CODED !!!
            readout.Clean();
            Thread.Sleep(50); //!!! HARD CODED !!!
            readout.Clean();
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
