using System.Xml.Serialization;

namespace Matura_2023_A2.Kakuro
{
    public class Sums
    {
        [XmlAttribute]
        public int X { get; set; }

        [XmlAttribute]
        public int Y { get; set; }

        [XmlAttribute]
        public int Horizontal { get; set; }

        [XmlAttribute]
        public int Vertical { get; set; }
    }
}
