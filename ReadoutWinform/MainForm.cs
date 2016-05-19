using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReadOutTestConsole;


namespace ReadoutWinform
{
    public partial class MainForm : Form
    {
        public List<double> Frequency;
        //private DataContainer tmpcontainer;
        private DCWrapper tmpcontainer;
        private int tmpcontainerpos;
        private Queue<double> displayqueue;

        private int softwareDSCount = 10;

        private bool keepRunning;
        private const int dc_length = 1000;

        private void startMeasurement()
        {            
            if (keepRunning)
                return;            
                        
            var ow = OutputWave.Instance;
            ow.SetFrequency(0, Frequency[0], 45.0, false);
            ow.SetFrequency(1, Frequency[1], 225.0, true);

            var rbcp = RBCP.Instance;
            var readout = Readout.Instance;

            keepRunning = true;
            
            while (keepRunning)
            {                
                var dcwrapper = new DCWrapper(dc_length);
                dcwrapper.SDCount = softwareDSCount;
                Program.DataContainers.Add(dcwrapper);
                dcwrapper.StartDateTime = DateTime.Now;

                rbcp.ToggleIQDataGate(true);
                readout.Read(dcwrapper, dcwrapper.Length);
                rbcp.ToggleIQDataGate(false);
            }            
            //StartButton.Enabled = true;
        }

        public MainForm()
        {            
            Frequency = new List<double>();
            displayqueue = new Queue<double>();
            // For DEBUG
            Frequency.Add(6.55e6); // resonance
            Frequency.Add(10e6); // off resonance
            // For DEBUG FIN
            InitializeComponent();

            keepRunning = false;
            DataViewPanel.Paint += new PaintEventHandler(PanelDraw);
        }

        private void PanelDraw( object sender, PaintEventArgs e)
        {
            if (displayqueue.Count == 0)
                return;
            var g = e.Graphics;
            var max = displayqueue.Max();
            var min = displayqueue.Min();
            var factor = (double)DataViewPanel.Height / (max-min);
            var points = displayqueue.Select((v, i) => new Point(i, (int)(DataViewPanel.Height - factor * (v - min))));
            g.DrawLines(Pens.Black, points.ToArray());            
        }

        private void DisplayQueueManipulation()
        {
            var newpoints = tmpcontainer.SDIndex - tmpcontainerpos;
            Enumerable.Range(tmpcontainerpos, newpoints).ToList().ForEach(i =>
            {
                var tmpi = tmpcontainer.SDIQArray[0].Is[i];
                var tmpq = tmpcontainer.SDIQArray[0].Qs[i];
                displayqueue.Enqueue(Math.Sqrt(tmpi * tmpi + tmpq * tmpq));
                if (displayqueue.Count > DataViewPanel.Width)
                    displayqueue.Dequeue();
            });
            tmpcontainerpos = tmpcontainer.SDIndex;
        }

        private void DisplayRefreshTimer_Tick(object sender, EventArgs e)
        {
            var nowcontainer = (DCWrapper)Program.DataContainers.Last();
            if (tmpcontainer == null)
                tmpcontainer = nowcontainer;
            
            tmpcontainer.Convert();
            tmpcontainer.SoftwareDownsample();
            bool toberefreshed = false;
            if(tmpcontainer.SDIndex != tmpcontainerpos)
            {
                DisplayQueueManipulation();
                toberefreshed = true;
            }
            if (nowcontainer != tmpcontainer)
            {
                tmpcontainer = nowcontainer;
                tmpcontainerpos = 0;
                DisplayQueueManipulation();
                toberefreshed = true;
            }
            if(toberefreshed)
                DataViewPanel.Refresh();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            DisplayRefreshTimer.Start();
            Thread t = new Thread(new ThreadStart(startMeasurement));
            t.IsBackground = true;
            t.Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            keepRunning = false;
        }
    }
}
