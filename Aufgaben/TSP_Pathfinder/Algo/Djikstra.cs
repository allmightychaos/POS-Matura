using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Pathfinder.Algo
{
    public class Djikstra
    {
        // 0 - Anfang/Ende
        private Stadt anfangsStadt;
        private Stadt endStadt;

        // 1 - Listen
        private List<Knoten> todolist;
        private HashSet<Stadt> erledigt;

        public Djikstra(List<Stadt> staedte)
        {
            this.anfangsStadt = staedte.First();
            this.endStadt = staedte.Last();
        }

        public void initialize()
        {
            todolist = new List<Knoten>();
            erledigt = new HashSet<Stadt>();

            // 2 - Startknoten
            Knoten startKnoten = new Knoten()
            {
                stadt = anfangsStadt,
                vorgaenger = null,
                gcost = 0,
                // hcost nur bei a* 
            };


            // 3 - zur todoliste
            todolist.Add(startKnoten);
        }

        public List<Stadt> MainLoop()
        {
            // 4 - Schleife

            while (todolist.Count > 0)
            {
                // a. - holen: aktuell billigsten knoten holen
                Knoten aktuell = todolist.OrderBy(o => o.fcost).FirstOrDefault();


                // b. - prüfen: zielstadt erreicht? 
                if (aktuell.stadt == endStadt)
                {
                    Knoten last = aktuell;
                    List<Stadt> pfad = new List<Stadt>();

                    while (last != null)
                    {
                        pfad.Add(last.stadt);
                        last = last.vorgaenger;
                    }

                    pfad.Reverse();
                    return pfad;
                }

                // c. - abhaken: von der liste
                todolist.Remove(aktuell);
                erledigt.Add(aktuell.stadt);


                // d. - nachbarn: nachbar überprüfen
                foreach (Stadt nachbar in aktuell.stadt.nachbarn)
                {
                    if (erledigt.Contains(nachbar)) continue;

                    // e. - updaten: neue distanz errechnen, ggf. updaten
                    double distanz = aktuell.gcost + calcDistance(aktuell.stadt, nachbar);
                    Knoten nachbarKnoten = todolist.FirstOrDefault(f => f.stadt == nachbar);

                    // A - kennen nachbar noch nicht
                    if (nachbarKnoten == null)
                    {
                        nachbarKnoten = new Knoten()
                        {
                            stadt = nachbar,
                            vorgaenger = aktuell,
                            gcost = distanz,
                            // hcost = calcDistance(nachbar, endStadt) 
                        };

                        todolist.Add(nachbarKnoten);
                    }

                    // B - kennen nachbar 
                    else if (distanz < nachbarKnoten.gcost)
                    {
                        nachbarKnoten.vorgaenger = aktuell;
                        nachbarKnoten.gcost = distanz;
                    }

                }

            }

            // wenn nichts gefunden null returnen
            return null;
        }

        // eukl distanz
        public double calcDistance(Stadt von, Stadt zu)
        {
            return Math.Sqrt(Math.Pow(zu.posX - von.posX, 2) + Math.Pow(zu.posY - von.posY, 2));
        }
    }
}
