using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk
{
    public class SampleDataModel
    {
        public int ID { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public double Probability { get; set; }
        public double Value { get; set; }

        //// just for testing
        //public override string ToString()
        //{
        //    return Id;
        //}
    }
}
