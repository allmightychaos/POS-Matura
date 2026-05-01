using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taschenrechner_vereinfacht.Grammatik
{
    public interface IExpression
    {
        public double Interpret();
    }
}
