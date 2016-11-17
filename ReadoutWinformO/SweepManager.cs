using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadoutWinformO;

namespace ReadoutConsole
{
    class SweepManager
    {
        private static SweepManager instance;
        public List<double[]> SweepFrequency;
        public int SweepPoints;
        public int NumberOfSamples; // for each frequency
        public List<List<double>> Is;
        public List<List<double>> Qs;

        private DataContainer datacontainer;

        public static SweepManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SweepManager();
                }
                return instance;
            }            
        }        

        private SweepManager()
        {
            NumberOfSamples = 10;
            Is = new List<List<double>>();
            Qs = new List<List<double>>();
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i => { Is.Add(new List<double>()); Qs.Add(new List<double>()); });
        }

        public void Clear()
        {
            Is = new List<List<double>>();
            Qs = new List<List<double>>();
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i => { Is.Add(new List<double>()); Qs.Add(new List<double>()); });
        }

        public void SetFrequency(double[] start_f, double[] df, int count)
        {            
            if (start_f.Count() != df.Count())
                throw new Exception("Array-size mismatch");

            SweepFrequency = new List<double[]>();
            SweepPoints = count;
            Enumerable.Range(0, start_f.Count()).ToList().ForEach(i =>
            {
                SweepFrequency.Add(Enumerable.Range(0, count).Select(j => start_f[i] + j * df[i]).ToArray());
            });
        }

        public void Sweep()
        {
            /* ONLY VALID FOR 2 CH READ-OUT */            
            var ow = OutputWave.Instance;
            var rbcp = RBCP.Instance;
            var readout = Readout.Instance;
            var ds = DownSample.Instance;
            
            Enumerable.Range(0, SweepPoints).ToList().ForEach(f_index =>
            {
                datacontainer = new DataContainer(NumberOfSamples);
                /*
                ow.SetFrequency(0, SweepFrequency[0][f_index], dds_en: false);
                if (Program.MY_DEBUG)                
                    Console.WriteLine("{0}", ow.GetFrequency(0));                
                ow.SetFrequency(1, SweepFrequency[1][f_index], dds_en: true);
                if (Program.MY_DEBUG)
                    Console.WriteLine("{0}", ow.GetFrequency(1));
                */
                Enumerable.Range(0, 64).ToList().ForEach(ch =>
                {
                    ow.SetFrequency(ch, SweepFrequency[ch%2][f_index], dds_en: false);
                });
                rbcp.DDSEnable();
                //ds.SetRate((int)(Program.DownSampleRate));

                readout.Clean();
                System.Threading.Thread.Sleep(50);
                readout.Clean();
                System.Threading.Thread.Sleep(50);
                rbcp.TimerReset();
                rbcp.ToggleIQDataGate(true);                                
                readout.Read(datacontainer, NumberOfSamples);
                rbcp.ToggleIQDataGate(false);
                //
                datacontainer.Convert();
                if (datacontainer.data[0] != 0xff)
                    throw new Exception("Header broken");                                

                Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(ch =>
                {
                    /*
                    // get median
                    int halfIndex = datacontainer.ConvertedLength / 2;
                    var sortedIs = datacontainer.IQArray[ch].Is.OrderBy(n => n);
                    var sortedQs = datacontainer.IQArray[ch].Qs.OrderBy(n => n);
                    double tmpi, tmpq;
                    if ((halfIndex % 2) == 0)
                    {
                        tmpi = (sortedIs.ElementAt(halfIndex) + sortedIs.ElementAt(halfIndex - 1)) / 2.0;
                        tmpq = (sortedQs.ElementAt(halfIndex) + sortedQs.ElementAt(halfIndex + 1)) / 2.0;
                    }
                    else
                    {
                        tmpi = sortedIs.ElementAt(halfIndex);
                        tmpq = sortedQs.ElementAt(halfIndex);
                    }
                    */
                    // add
                    Is[ch].Add(datacontainer.IQArray[ch].Is.Average());
                    Qs[ch].Add(datacontainer.IQArray[ch].Qs.Average());
                });

                if (Program.Verbose)
                {
                    Console.WriteLine("{0} {1} {2} {3} {4} {5}",
                                       SweepFrequency[0][f_index],
                                       SweepFrequency[1][f_index],
                                       Is[0][f_index],
                                       Is[1][f_index],
                                       Qs[0][f_index],
                                       Qs[1][f_index]);
                }
                                                                                        
            });

            if (Program.MY_DEBUG)
            {
                /*
                using (var sw = new System.IO.StreamWriter("sweep.txt"))
                {
                    Enumerable.Range(0, SweepPoints).ToList().ForEach(f_index =>
                    {
                        sw.WriteLine("{0} {1} {2} {3} {4}");
                    });
                }
                */
            }
        }        
    }
}
