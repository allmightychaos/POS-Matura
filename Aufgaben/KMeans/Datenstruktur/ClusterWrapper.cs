using System.Windows;
using System.Windows.Media;

namespace KMeans.Datenstruktur
{
    public class ClusterWrapper
    {
        public int clusterId { get; set; }
        public Point centroid { get; set; }
        public Brush brushColor { get; set; }
    }
}
