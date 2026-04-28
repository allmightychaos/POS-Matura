using System.Xml.Serialization;

namespace KMeans.Datenstruktur
{
    [XmlRoot]
    public class People
    {
        [XmlElement]
        public List<Person> Person { get; set; } = new List<Person>();
    }

    public class Person
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public double PixelX { get; set; }

        [XmlElement]
        public double PixelY { get; set; }


        // optional
        public int? clusterID { get; set; }
    }
}
