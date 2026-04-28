using System.Collections.Generic;

namespace A1.Grammatik
{
    public class ImplicationExpression : AbstractExpression
    {
        private AbstractExpression _left;
        private AbstractExpression _right;

        public ImplicationExpression(AbstractExpression left, AbstractExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Interpret(Dictionary<char, bool> context)
        {
            return !_left.Interpret(context) || _right.Interpret(context);
        }
    }
}
