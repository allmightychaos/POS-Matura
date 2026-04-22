using System.Collections.Generic;

namespace A1.Grammatik
{
    public class VariableExpression : AbstractExpression
    {
        private char _name;

        public VariableExpression(char name)
        {
            _name = name;

            // Füge die Veriable der statischen Liste hinzu, falls noch nicht enthalten
            if (!variables.Contains(name))
            {
                variables.Add(name);
            }
        }

        public override bool Interpret(Dictionary<char, bool> context)
        {
            // Holt den Wahrheitswert (true/false) für _name
            return context[_name];
        }
    }
}
