using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taschenrechner_vereinfacht.Grammatik
{
    public class BinaryExpression : IExpression
    {
        private IExpression _left;
        private IExpression _right;
        string op;

        public BinaryExpression(IExpression left, IExpression right, string op)
        {
            this._left = left;
            this._right = right;
            this.op = op;
        }

        public double Interpret()
        {
            return op switch
            {
                "+" => _left.Interpret() + _right.Interpret(),
                "-" => _left.Interpret() - _right.Interpret(),
                "*" => _left.Interpret() * _right.Interpret(),
                "/" => _left.Interpret() / _right.Interpret(),
                "^" => Math.Pow(_left.Interpret(), _right.Interpret()),
            };
        }
    }
}
