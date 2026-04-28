using System.Collections.Generic;
using System.Xml.Serialization;

namespace Matura_2023_A2.Kakuro
{
    [XmlRoot("Kakuro")]
    public class KakuroData
    {
        [XmlAttribute("Rows")]
        public int Rows { get; set; }

        [XmlAttribute("Columns")]
        public int Cols { get; set; }

        // ============================ \\

        [XmlArrayItem("Field")]
        public List<InaktiveFields> InaktiveFields { get; set; }

        [XmlArrayItem("Sum")]
        public List<Sums> Sums { get; set; }
    }
}
