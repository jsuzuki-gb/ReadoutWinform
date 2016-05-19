using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadOutTestConsole;

namespace ReadoutWinform
{
    class DCWrapper : DataContainer
    {
        // SD means Software Downsampling
        public int SDCount;
        public IQInfo[] SDIQArray;
        public int SDIndex;

        public DCWrapper(int length) : base(length)
        {
            SDCount = 1;
            SDIndex = 0;
            SDIQArray = new IQInfo[Program.NumberOfChannels];
            Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i => { SDIQArray[i] = new IQInfo(); });
        }

        public void SoftwareDownsample()
        {
            if (SDIndex == ConvertedLength/SDCount )
                return;
            for (; SDIndex < ConvertedLength / SDCount; SDIndex++)
            {
                Enumerable.Range(0, Program.NumberOfChannels).ToList().ForEach(i => {
                    var iq = SDValue(SDIndex, i);
                    SDIQArray[i].Is.Add(iq[0]);
                    SDIQArray[i].Qs.Add(iq[1]);
                });
            }            
        }

        private double[] SDValue(int sd_index, int channel)
        {
            double tmpI = 0;
            double tmpQ = 0;
            for(int i = 0; i < SDCount; i++)
            {
                tmpI += this.IQArray[channel].Is[sd_index * SDCount + i];
                tmpQ += this.IQArray[channel].Qs[sd_index * SDCount + i];
            }
            return new double[]{tmpI/SDCount, tmpQ/SDCount};
        }
    }
}
