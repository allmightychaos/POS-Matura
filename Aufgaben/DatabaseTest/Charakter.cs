using Microsoft.Data.Sqlite;
using System.Linq;
using LinqToDB.Mapping;

namespace DatabaseTest
{
    [Table("Character")]
    internal class Charakter
    {
        [Column("ID", IsPrimaryKey = true, SkipOnInsert = true, SkipOnUpdate = true)]
        public int id {  get; set; }

        [Column("CharacterName")]
        public string name { get; set; }

        [Column("Klasse")]
        public string klasse { get; set; }

        [Column("Level")]
        public int level { get; set; }
    }
}
