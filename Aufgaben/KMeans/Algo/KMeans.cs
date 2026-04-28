using System.Windows;
using KMeans.Datenstruktur;

namespace KMeans.Algo
{
    public class KMeans
    {
        public List<Person> people;
        public Point[] centroid;
        public int status = 0; // 0 => nothing || 1 => initialized || 2 => distance calc || 3 => centroid updated

        public KMeans(List<Person> people, int clusterCount)
        {
            this.people = people;
            centroid = new Point[clusterCount];
        }

        public void InitializeCentroids(Random random)
        {
            for (int i = 0; i < centroid.Length; i++)
            {
                var index = random.Next(people.Count());
                var personPos = people[index];

                Point p = new Point()
                {
                    X = people[index].PixelX,
                    Y = people[index].PixelY,
                };

                centroid[i] = p;
            }


            status = 1; // initialized
        }

        public bool calcDistance()
        {
            int counter = 0;

            foreach (var person in people)
            {
                double distance = double.MaxValue;
                int bestCluster = -1;
                int oldCluster = person.clusterID.GetValueOrDefault(-1);

                for (int i = 0; i < centroid.Length; i++)
                {
                    double dX = person.PixelX - centroid[i].X;
                    double dY = person.PixelY - centroid[i].Y;

                    double dSquared = dX * dX + dY * dY;

                    if (dSquared < distance)
                    {
                        distance = dSquared;
                        bestCluster = i;
                    }
                }

                if (bestCluster != oldCluster)
                {
                    counter++;
                }

                person.clusterID = bestCluster;
            }

            status = 2; // distance calculated

            // converged (if true)
            return counter == 0;
        }

        public void updateCentroid()
        {
            for (int i = 0; i < centroid.Length; i++)
            {
                // grab list of distances from people with the clusterID of i
                var dList = people.Where(w => w.clusterID == i).ToList();

                // if dList is empty, continue (=> no people for this cluster)
                if (dList.Count == 0) continue;

                // set new centroid position
                centroid[i].X = dList.Average(a => a.PixelX);
                centroid[i].Y = dList.Average(a => a.PixelY);
            }

            status = 3; // centroids updated
        }
    }
}
