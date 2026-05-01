using LinqToDB;
using LinqToDB.Data;

namespace DatabaseTest
{
    public class TestDb : DataConnection
    {
        public TestDb(DataOptions<TestDb> options) : base(options.Options) { }

        public ITable<Charakter> Charakter => this.GetTable<Charakter>();
    }
}
