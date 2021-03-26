using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFGUI
{
    public class AddMeasurement
    {
        public long AMKA { get; set; }
        public string Date { get; set; }
        // you do not have to put every column here but it is better to 
        public string FileName{ get; set; }
        public long PK { get; set; }
        public int Neuron { get; set; }
        public int Side { get; set; }
        public float Mlat { get; set; }
        public float Marea { get; set; }
        public float Mamp { get; set; }
        public float Fper { get; set; }
        public float FRepPer { get; set; }
        public int RNs { get; set; }
        public int RN5Rep { get; set; }
        public int FRepsTotal { get; set; }

    }
}
