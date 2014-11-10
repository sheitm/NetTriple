using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTriple.Tests.TestDomain
{
    public class MeterReading
    {
        public string Mpt { get; set; }
        public string Msno { get; set; }
        public IEnumerable<Sr> Series { get; set; } 
    }

    public class Sr
    {
        public string Rty { get; set; }
        public string Vty { get; set; }
        public string Un { get; set; }
        public IEnumerable<Mv> MeterValues { get; set; } 

        // Added in partial
        public string Mpt { get; set; }
        public string Msno { get; set; }

        private string _id;
        public string Id
        {
            get
            {
                if (_id == null)
                {
                    var dt = MeterValues.First().Ts;
                    _id = string.Format("{0}_{1}_{2}_{3}_{4}-{5}-{6}",
                        Mpt,
                        Rty,
                        Vty, Un,
                        dt.Year,
                        dt.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                        dt.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
                }

                return _id;
            }
            set { _id = value; }
        }

        public string Subject
        {
            get { return Id; }
        }
    }

    public class Mv
    {
        public decimal Val { get; set; }
        public DateTime Ts { get; set; }
        public int Interval { get; set; }
        public string Cm { get; set; }
        public string Sts { get; set; }
        public string Cst { get; set; }
    }
}
