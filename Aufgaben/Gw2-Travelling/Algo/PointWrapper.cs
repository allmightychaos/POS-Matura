using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gw2_Travelling.Algo
{
    public class PointWrapper
    {
        public Point point { get; set; }
        public PointWrapper vorgaenger { get; set; }
        public List<PointWrapper> nachbarn {  get; set; }
    }
}
