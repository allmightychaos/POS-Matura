using LinqToDB.Mapping;

namespace Osterhasen
{
    [Table("Person")]
    public class Person
    {
        [PrimaryKey, Identity]
        public int PersonenID { get; set; }

        [Column]
        public string name { get; set; }
        [Column]
        public double lng { get; set; }
        [Column]
        public double lat { get; set; }
    }
}
