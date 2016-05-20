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

        private bool keepRunning;
        private const int dc_length = 1000;

        // TOD Display settings
        private int xAxisDisplaySpacing;
        private bool yAxisFix;
        private double yAxisTop;
        private double yAxisBottom;

        public MainForm()
        {
            Frequency = new List<double>();
            displayqueue = new Queue<double>();

            // TOD display settings
            xAxisDisplaySpacing = 1;
            yAxisFix = false;
            yAxisTop = 100;
            yAxisBottom = 0;

            // For DEBUG
            Frequency.Add(6.55e6); // resonance
            Frequency.Add(10e6); // off resonance
            // For DEBUG FIN
            InitializeComponent();
            ShowSettings();

            keepRunning = false;
            DataViewPanel.Paint += new PaintEventHandler(PanelDraw);
            ReadoutStatus.Text = "Ready";
        }

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

            readout.Clean();
            Thread.Sleep(50); //!!! HARD CODED !!!
            readout.Clean();

            rbcp.ToggleIQDataGate(true);
            while (keepRunning)
            {                
                var dcwrapper = new DCWrapper(dc_length);
                dcwrapper.SDCount = Program.SoftwareDownsampleCount;
                Program.DataContainers.Add(dcwrapper);
                dcwrapper.StartDateTime = DateTime.Now;                                
                readout.Read(dcwrapper, dcwrapper.Length);                
            }
            rbcp.ToggleIQDataGate(false);
            //StartButton.Enabled = true;
        }

        public void ShowSettings()
        {
            SDTextBox.Text = Program.SoftwareDownsampleCount.ToString();
            xAxisDisplaySpacingTextBox.Text = xAxisDisplaySpacing.ToString();
            YAxisFixComboBox.SelectedIndex = Convert.ToInt32(!yAxisFix);
            yTopTextBox.Text = yAxisTop.ToString();
            yBottomTextBox.Text = yAxisBottom.ToString();
            if (!yAxisFix)
            {
                yTopTextBox.Enabled = false;
                yBottomTextBox.Enabled = false;
            }
            freqTextBox1.Text = Frequency[0].ToString();            
            freqTextBox2.Text = Frequency[1].ToString();
        }

        public void ReadSettings()
        {            
            try
            {
                Program.SoftwareDownsampleCount = Convert.ToInt32(SDTextBox.Text);
                xAxisDisplaySpacing = Convert.ToInt32(xAxisDisplaySpacingTextBox.Text);
                yAxisFix = Convert.ToBoolean(YAxisFixComboBox.Text);
                if (yAxisFix)
                {
                    yAxisTop = Convert.ToDouble(yTopTextBox.Text);
                    yAxisBottom = Convert.ToDouble(yBottomTextBox.Text);                    
                }
                Frequency[0] = Convert.ToDouble(freqTextBox1.Text);
                Frequency[1] = Convert.ToDouble(freqTextBox2.Text);
            } catch (Exception)
            {
                MessageBox.Show("Parse error", "Parse error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void PanelDraw( object sender, PaintEventArgs e)
        {
            if (displayqueue.Count == 0)
                return;
            var g = e.Graphics;
            if (!yAxisFix)
            {
                yAxisTop = displayqueue.Max();
                yAxisBottom = displayqueue.Min();
            }            
            var factor = (double)DataViewPanel.Height / (yAxisTop-yAxisBottom);
            var points = displayqueue.Select((v, i) => new Point(i*xAxisDisplaySpacing, (int)(DataViewPanel.Height - factor * (v - yAxisBottom))));
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
                while (displayqueue.Count > DataViewPanel.Width/xAxisDisplaySpacing)
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
            tmpcontainer.SoftwareDownsample(true);
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
            ReadoutStatus.Text = "Running...";
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            keepRunning = false;
            ReadoutStatus.Text = "Stopped";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Program.GUI_DEBUG)
            {
                RBCP.Instance.Close();
                Readout.Instance.Close();
            }            
        }

        private void YAxisFixComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (YAxisFixComboBox.Text == "")
                return;
            if (Convert.ToBoolean(YAxisFixComboBox.Text))
            {
                yTopTextBox.Enabled = true;
                yBottomTextBox.Enabled = true;
            }
            else
            {
                yTopTextBox.Enabled = false;
                yBottomTextBox.Enabled = false;
            }
        }

        private void settingApplyButton_Click(object sender, EventArgs e)
        {
            ReadSettings();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "hoge.dat";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            sfd.Filter = "All Files | *.*";
            sfd.FilterIndex = 0;
            sfd.Title = "Save";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using ( var stream = sfd.OpenFile() )
                using ( System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
                {
                    foreach (var dc in Program.DataContainers)
                    {
                        dc.Express().ForEach(line => sw.WriteLine(line));
                    }
                }
            }
        }
    }
}
