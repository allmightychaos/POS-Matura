using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1.Grammatik
{
    public class LogicParser : AbstractExpression
    {
        private AbstractExpression _root;

        public override void ParseFormel(List<char> formel)
        {
            _root = ParseAusdruck(formel);
        }

        public override bool Interpret(Dictionary<char, bool> context)
        {
            return _root.Interpret(context);
        }

        public AbstractExpression ParseAusdruck(List<char> formel)
        {
            var links = ParseNegation(formel);

            char op = Peek(formel);

            if (op == '&' || op == '|' || op == '>' || op == '=')
            {
                Pop(formel);

                var rechts = ParseAusdruck(formel);

                switch (op)
                {
                    case '&':
                        return new AndExpression(links, rechts);
                    case '|':
                        return new OrExpression(links, rechts);
                    case '>':
                        return new ImplicationExpression(links, rechts);
                    case '=':
                        return new EquivalenceExpression(links, rechts);

                    default:
                        return links;
                }
            }

            return links;
        }

        public AbstractExpression ParseNegation(List<char> formel)
        {
            if (Peek(formel) == '!')
            {
                Pop(formel);

                // Rekursiv weiter für `!!p`
                return new NotExpression(ParseNegation(formel));
            }

            return ParseAtom(formel);
        }

        public AbstractExpression ParseAtom(List<char> formel)
        {
            if (Peek(formel) == '(')
            {
                Pop(formel); // '(' entfernen

                var expr = ParseAusdruck(formel);

                Pop(formel); // ')' entfernen
                return expr;
            }
            
            // Es muss ein buchstabe sein (a-z)
            char buchstabe = Pop(formel);
            return new VariableExpression(buchstabe);
        }



        char Peek(List<char> formel)
        {
            if (formel.Count > 0)
            {
                return formel[0];
            }

            return '\0';
        }

        char Pop(List<char> formel)
        {
            char c = Peek(formel);
            formel.RemoveAt(0);

            return c;
        }
    }
}
