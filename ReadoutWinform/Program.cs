using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReadOutTestConsole;

namespace ReadoutWinform
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static bool GUI_DEBUG = false;

        public static bool MY_DEBUG = true;
        public static int NumberOfChannels = 2;
        public static double ADCSampleRate = 200e6;
        public static double DownSampleRate = 2e5;
        public static int DataUnit = NumberOfChannels * 14 + 7;
        public static bool Verbose = true;
        public static int SoftwareDownsampleCount = 20;

        public static List<DataContainer> DataContainers;

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

            var adc_control = ADCControl.Instance;
            var dac_control = DACControl.Instance;

            // register init
            adc_control.Clean();
            dac_control.Clean();
            adc_control.ADCRegisterInitialize();
            dac_control.DACRegisterInitialize();

            readout.Connect();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if(!GUI_DEBUG)
                init();
            DataContainers = new List<DataContainer>();

            Application.Run(new MainForm());
        }
    }
}
