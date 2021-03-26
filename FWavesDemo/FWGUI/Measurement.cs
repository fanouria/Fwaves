using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFGUI
{
    public class Measurement
    {
        public long AMKA { get; set; }
        public long WavePK { get; set; }
        public float WaveID { get; set; }
        // you do not have to put every column here but it is better to 
        public float Latency { get; set; }
        public float Duration { get; set; }
        public float Amplitude { get; set; }
        public float Area { get; set; }
        public float FCV { get; set; }
    }

}
