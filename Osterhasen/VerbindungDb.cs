using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;

namespace Osterhasen
{
    public class VerbindungDb : DataConnection
    {
        public VerbindungDb(DataOptions<VerbindungDb> options) : base(options.Options) { }

        public ITable<Person> Person => this.GetTable<Person>();
    }
}
