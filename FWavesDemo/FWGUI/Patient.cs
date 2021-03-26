using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI
{
    public class Patient
    {
        public long AMKA { get; set; }
        public long Birth { get; set; }
        public int Sex { get; set; }
        public float Height { get; set; }
        public float ArmLength { get; set; }
        public float LegLength { get; set; }
        public int Diagnosis{ get; set; }
        public string Comment { get; set; }

        //public string FullInfo
        //{
        //    get
        //    {
        //        // " Fanouria Athanasiou (test@test.com)
        //        return $"am:{AMKA}  Sex:{Sex}  Height:{Height}cm  Arm Length:{ArmLength}cm";
        //    }

        //}
    }
}
