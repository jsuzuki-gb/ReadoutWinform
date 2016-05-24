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
using System.Numerics;
using MathNet.Numerics;


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
        private bool measuring;
        private const int dc_length = 1000;

        // TOD Display settings
        private int xAxisDisplaySpacing;
        private bool yAxisFix;
        private double yAxisTop;
        private double yAxisBottom;

        private int xGridNumber; // TO BE IMPLEMENTED
        private int yGridNumber; // TO BE IMPLEMENTED
        private int xGridTmpPos;
        private Font gridFont = new Font("MS UI Gothic", 10);

        // for sweep
        private double sweepStartFreq;
        private double sweepEndFreq;
        private int sweepCount;

        private List<double> tmpIs;
        private List<double> tmpQs;
        private List<Complex> tmpCs;
        private List<double> tmpabs;
        private double sweepAbsMax;
        private double sweepAbsMin;

        public MainForm()
        {
            Frequency = new List<double>();
            displayqueue = new Queue<double>();

            // TOD display settings
            xAxisDisplaySpacing = 1;
            yAxisFix = false;
            yAxisTop = 100;
            yAxisBottom = 0;
            xGridNumber = 3;
            yGridNumber = 3;
            xGridTmpPos = 0;

            sweepCount = 101;
            

            // For DEBUG
            Frequency.Add(6.55e6); // resonance
            Frequency.Add(10e6); // off resonance
            // For DEBUG FIN
            InitializeComponent();
            axisPanel.Controls.Add(DataViewPanel);            
            ShowSettings();

            keepRunning = false;
            measuring = false;
            DataViewPanel.Paint += new PaintEventHandler(PanelDraw);
            axisPanel.Paint += new PaintEventHandler(axDraw);
            sweepAbsPanel.Paint += new PaintEventHandler(sweepAbsDraw);
            sweepUpperIQPanel.Paint += new PaintEventHandler(sweepUpperIQDraw);
            ReadoutStatus.Text = "Ready";
        }

        private void startMeasurement()
        {            
            if (measuring)
                return;            
                        
            var ow = OutputWave.Instance;
            ow.SetFrequency(0, Frequency[0], 45.0, false);
            ow.SetFrequency(1, Frequency[1], 225.0, true);

            var rbcp = RBCP.Instance;
            var readout = Readout.Instance;

            keepRunning = true;
            measuring = true;

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

            measuring = false;
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

            // sweep panel
            sweepStartFreqTextbox.Text = (Frequency[0] - 1e6).ToString();
            sweepEndFreqTextbox.Text = (Frequency[0] + 1e6).ToString();
            sweepCountTextbox.Text = sweepCount.ToString();
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
            var factor = DataViewPanel.Height / (yAxisTop-yAxisBottom);
            var points = displayqueue.Select((v, i) => new Point(i*xAxisDisplaySpacing, (int)(DataViewPanel.Height - factor * (v - yAxisBottom))));
            g.DrawLines(Pens.Blue, points.ToArray());

            // Grids
            var g_pen = new Pen(Color.Red, 1);
            g_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            for(int i = 0; i < xGridNumber; i++)
            {
                var tmploc = (xGridTmpPos + DataViewPanel.Width / xGridNumber * i) % DataViewPanel.Width;
                var red_tmploc = (tmploc + DataViewPanel.Width) % DataViewPanel.Width;
                g.DrawLine(g_pen, new Point(red_tmploc, DataViewPanel.Height), new Point(red_tmploc, 0));
            }
            for(int i = 0; i < yGridNumber; i++)
            {
                var tmploc = DataViewPanel.Height / (yGridNumber + 1) * (i + 1);
                var realval = (DataViewPanel.Height - tmploc) / factor + yAxisBottom;
                g.DrawLine(g_pen, new Point(0, tmploc), new Point(DataViewPanel.Width, tmploc));
                g.DrawString(realval.ToString(), gridFont, Brushes.Red, 0, tmploc + 10);
            }
        }

        private void axDraw( object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var vertical_top = new Point(DataViewPanel.Location.X - axisPanel.Location.X, 0);
            var vertical_bottom = new Point(DataViewPanel.Location.X - axisPanel.Location.X, axisPanel.Height);
            var horizontal_left = new Point(0, DataViewPanel.Height);
            var horizontal_right = new Point(axisPanel.Width, DataViewPanel.Height);
            g.DrawLine(Pens.Black, vertical_top, vertical_bottom);
            g.DrawLine(Pens.Black, horizontal_left, horizontal_right);
        }

        private void DisplayQueueManipulation()
        {
            var newpoints = tmpcontainer.SDIndex - tmpcontainerpos;
            Enumerable.Range(tmpcontainerpos, newpoints).ToList().ForEach(i =>
            {
                var tmpi = tmpcontainer.SDIQArray[0].Is[i];
                var tmpq = tmpcontainer.SDIQArray[0].Qs[i];
                displayqueue.Enqueue(Math.Sqrt(tmpi * tmpi + tmpq * tmpq));
                while (displayqueue.Count > DataViewPanel.Width / xAxisDisplaySpacing)
                {
                    displayqueue.Dequeue();
                    xGridTmpPos -= xAxisDisplaySpacing;
                }
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
            if (measuring)
                return;
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

        private void refreshButton_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void sweepAbsDraw(object sender, PaintEventArgs e)
        {
            var sm = SweepManager.Instance;
            if (sm.Is[0].Count <= 1)
                return;

            var g = e.Graphics;            
            var points = tmpabs.Select((v, index) =>
            {
                var tmpfreq = sm.SweepFrequency[0][index];
                var xpos = (tmpfreq - sweepStartFreq) / (sweepEndFreq - sweepStartFreq) * sweepAbsPanel.Width;
                var ypos = (1- (v - sweepAbsMin) / (sweepAbsMax - sweepAbsMin)) * sweepAbsPanel.Height;
                return new Point((int)xpos, (int)ypos);
            });
            g.DrawLines(Pens.Black, points.ToArray());
        }

        private void sweepUpperIQDraw(object sender, PaintEventArgs e)
        {
            var sm = SweepManager.Instance;
            if (sm.Is[0].Count <= 1)
                return;

            var g = e.Graphics;
            var w = sweepUpperIQPanel.Width;
            var h = sweepUpperIQPanel.Height;
            var raw_points = tmpCs.Select(c => new Point((int)(c.Real / sweepAbsMax * w/2 + w/2),
                                                         (int)(c.Imaginary / sweepAbsMax * h/2 + h/2)));
            g.DrawLines(Pens.Blue, raw_points.ToArray());
        }


        private void sweepStartButton_Click(object sender, EventArgs e)
        {
            if (measuring)
            {
                MessageBox.Show("Blocked", "Blocked by other processes",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                return;
            }                

            try
            {
                sweepStartFreq = Convert.ToDouble(sweepStartFreqTextbox.Text);
                sweepEndFreq = Convert.ToDouble(sweepEndFreqTextbox.Text);
                sweepCount = Convert.ToInt32(sweepCountTextbox.Text);
            }
            catch
            {
                MessageBox.Show("Parse error", "Parse error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
                return;
            }

            var sm = SweepManager.Instance;
            if(sm.Is[0].Count != 0)
                sm.Clear();

            sm.SetFrequency(new double[] { sweepStartFreq, Frequency[1] },
                            new double[] { (sweepEndFreq - sweepStartFreq) / sweepCount, 0 },
                            sweepCount);

            sweepDisplayRefreshTimer.Enabled = true;
            Thread t = new Thread(new ThreadStart(doSweep));
            t.IsBackground = true;
            t.Start(); 
        }

        private void doSweep()
        {
            var sm = SweepManager.Instance;
            measuring = true;
            sm.Sweep();
            measuring = false;
            // for debugging
            using(var sw = new System.IO.StreamWriter("sweep.dat"))
            {
                for (int i = 0; i < sm.SweepPoints; i++)
                {
                    sw.WriteLine("{0} {1} {2}", sm.SweepFrequency[0][i], sm.Is[0][i], sm.Qs[0][i]);
                }
            }            
        }

        private void sweepDisplayRefreshTimer_Tick(object sender, EventArgs e)
        {
            var sm = SweepManager.Instance;
            if (sm.Is[0].Count <= 1)
                return;
                        
            tmpIs = sm.Is[0].ToList();
            tmpQs = sm.Qs[0].ToList();
            tmpCs = tmpIs.Zip(tmpQs, Tuple.Create).Select(iq => new Complex(iq.Item1, iq.Item2)).ToList();
            tmpabs = tmpCs.Select(c => c.Magnitude).ToList();
            sweepAbsMax = tmpabs.Max();
            sweepAbsMin = tmpabs.Min();
            // Draw
            sweepAbsPanel.Refresh();
            sweepUpperIQPanel.Refresh();
        }

        private void Fit()
        {
            var sm = SweepManager.Instance;
            if (sm.Is[0].Count <= 1)
                return;


        }

        private void sweepFitButton_Click(object sender, EventArgs e)
        {
            Fit();
            sweepUpperIQPanel.Refresh();
            sweepBottomIQPanel.Refresh();
            
        }
    }
}
