using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Osterhasen.Algorithm
{
    public class K_Means
    {
        private List<DataPoint> points;
        public Point[] centroids {  get; set; }

        public K_Means(List<Person> people, int cluster)
        {
            points = new List<DataPoint>();

            toDataPoint(people);
            InitializeCentroids(cluster);

            while (true)
            {
                if (!calculateDistance())
                {
                    newCentroids();
                }
                else
                {
                    break;
                }
            }
        }

        // calculate shortest distance for each point and return if converged (returns true)
        public bool calculateDistance()
        {
            int counter = 0;
            foreach (var point in points)
            {
                int oldCluster = point.clusterID;
                int bestCluster = -1;
                double minDistance = double.MaxValue;

                for (int i = 0; i < centroids.Length; i++)
                {

                    // 1. Calculate distance
                    double dx = point.X - centroids[i].X;
                    double dy = point.Y - centroids[i].Y;

                    double dSquared = dx * dx + dy * dy;

                    if (dSquared < minDistance)
                    {
                        minDistance = dSquared;
                        bestCluster = i;
                    }

                }

                // assign the clusterId to the point
                point.clusterID = bestCluster;
                if (oldCluster != bestCluster) counter++;
            }

            return counter < 1;
        }

        public void newCentroids()
        {
            // k => cluster
            for (int i = 0; i < centroids.Length; i++)
            {
                var clusterPoints = points.Where(w => w.clusterID == i).ToList();

                // check if there are any points assigned to a cluster
                if (clusterPoints.Any())
                {

                    centroids[i].X = clusterPoints.Average(w => w.X);

                    centroids[i].Y = clusterPoints.Average(w => w.Y);
                }
            }
        }

        // ============================= HELPER ============================= \\
        public void InitializeCentroids(int cluster)
        {
            centroids = new Point[cluster];
            Random ran = new Random();

            for (int i = 0; i < cluster; i++)
            {
                int ranIndex = ran.Next(points.Count);

                centroids[i] = new Point
                {
                    X = points[ranIndex].X,
                    Y = points[ranIndex].Y,
                };
            }
        }
        
        public void toDataPoint(List<Person> people)
        {
            foreach (var person in people)
            {
                points.Add(new DataPoint()
                {
                    person = person,
                    X = person.lng,
                    Y = person.lat,
                    clusterID = 0
                });
            }
        }
    }
}
