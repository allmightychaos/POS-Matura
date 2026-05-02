using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;

namespace DatabaseTest
{
    internal class DbTest : DataConnection
    {
        public DbTest(DataOptions<DbTest> options) : base(options.Options) { }
    }
}
