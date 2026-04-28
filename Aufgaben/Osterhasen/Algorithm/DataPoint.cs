using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osterhasen.Algorithm
{
    public class DataPoint
    {
        public Person person { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int clusterID { get; set; }
    }
}
