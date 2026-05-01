using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Pathfinder.Algo
{
    public class Knoten
    {
        public Stadt stadt {  get; set; }
        public Knoten vorgaenger { get; set; }

        // distanz kosten
        public double gcost { get; set; } 
        // a* - heuristik kosten
        public double hcost { get; set; }

        // summe der kosten
        public double fcost => gcost + hcost;
    }
}
