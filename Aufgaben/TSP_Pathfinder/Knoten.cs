using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_WPF;

namespace TSP_Pathfinder
{
    public class Knoten
    {
        public City Stadt {  get; set; }
        public Knoten Vorgänger { get; set; }

        // G-Cost: bisher gelaufene Distanz vom Startknoten
        public double GCost { get; set; } = double.MaxValue;

        // H-Cost: geschätze Luftlinie zum Ziel (ignoriert von Djikstra, für A*)
        public double HCost { get; set; } = 0;

        // F-Cost: Summe aus G-&H-Cost zum sortieren für A*
        public double FCost => GCost + HCost;
    }
}
