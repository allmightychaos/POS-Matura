using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taschenrechner_vereinfacht.Grammatik
{
    public class UnaryExpression : IExpression
    {
        private IExpression _operand;

        public UnaryExpression(IExpression operand)
        {
            _operand = operand;
        }

        public double Interpret()
        {
            return -_operand.Interpret();
        }
    }
}
