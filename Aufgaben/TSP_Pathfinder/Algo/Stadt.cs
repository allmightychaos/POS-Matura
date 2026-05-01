using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Pathfinder.Algo
{
    public class Stadt
    {
        // Stadt
        public string name { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }

        // Djikstra / A*
        public List<Stadt> nachbarn {  get; set; }
    }
}
