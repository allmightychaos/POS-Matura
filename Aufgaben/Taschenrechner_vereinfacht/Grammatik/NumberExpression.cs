using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taschenrechner_vereinfacht.Grammatik
{
    public class NumberExpression : IExpression
    {
        double value;

        public NumberExpression(double value)
        {
            this.value = value;
        }

        public double Interpret()
        {
            return value;
        }
    }
}
