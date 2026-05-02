using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2_Travelling.Algo
{
    public class TravellingSalesman
    {
        List<Point> points;
        Point start;

        public TravellingSalesman(List<Point> points, Point? start = null)
        {
            this.points = points;

            // select random point as start if null
            if (start == null) 
            {
                Random rnd = new Random();
                this.start = points[rnd.Next(points.Count)];
            }
            else
            {
                this.start = (Point)start;
            }
        }

        public List<Point> Run()
        {
            return NearestNeighbor();
        }

        private List<Point> NearestNeighbor()
        {
            List<Point> verbleibend = points.ToList();
            List<Point> path = new List<Point>();

            verbleibend.Remove(start);
            path.Add(start);

            while (verbleibend.Count > 0)
            {
                // 1. Last Point holen
                Point last = path.Last();

                // 2. Nächst kürzesten berechnen
                Point next = verbleibend
                    .OrderBy(o => calcDistance(last, o))
                    .FirstOrDefault();

                // 3. Nächsten von verbleibend entfernen und zum Pfad
                verbleibend.Remove(next);
                path.Add(next);

                // 4. Repeat!
            }
            MessageBox.Show($"Salesman: {path.Count()}");

            return path;
        }

        // ============================ HELPER ============================ \\

        private double calcDistance(Point from, Point to)
        {
            return Math.Sqrt(Math.Pow(to.X - from.X, 2) + Math.Pow(to.Y - from.Y, 2));
        }

    }
}
