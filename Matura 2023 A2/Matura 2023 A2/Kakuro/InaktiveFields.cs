using System.Xml.Serialization;

namespace Matura_2023_A2.Kakuro
{
    public class InaktiveFields
    {
        [XmlAttribute]
        public int X { get; set; }

        [XmlAttribute]
        public int Y { get; set; }
    }
}
