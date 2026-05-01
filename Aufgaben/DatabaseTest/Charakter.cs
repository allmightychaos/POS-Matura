using System.Collections.Generic;
using LinqToDB.Mapping;

namespace DatabaseTest
{
    [Table("Charakter")]
    public class Charakter
    {
        [Column("id", IsPrimaryKey = true, IsIdentity = true, SkipOnInsert = true, SkipOnUpdate = true)]
        public int id { get; set; }

        [Column("CharakterName")]
        public string charName { get; set; }

        [Column("Klasse")]
        public string klasse { get; set; }

        [Column("Level")]
        public int level { get; set; }
    }
}
