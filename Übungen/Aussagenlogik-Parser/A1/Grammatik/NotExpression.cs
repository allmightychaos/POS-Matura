using System.Collections.Generic;

namespace A1.Grammatik
{
    public class NotExpression : AbstractExpression
    {
        private AbstractExpression _operand;

        public NotExpression(AbstractExpression operand)
        {
            _operand = operand;
        }

        public override bool Interpret(Dictionary<char, bool> context)
        {
            return !_operand.Interpret(context);
        }
    }
}
