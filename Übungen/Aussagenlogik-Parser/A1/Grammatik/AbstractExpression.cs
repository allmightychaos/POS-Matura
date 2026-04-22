using System.Collections.Generic;

namespace A1.Grammatik
{
    public abstract class AbstractExpression
    {
        public virtual void ParseFormel(List<char> formel)
        {

        }

        //Variablenliste wird beim Parsen befüllt
        public static List<char> variables = new List<char>();

        public virtual bool Interpret(Dictionary<char, bool> context)
        {
            return false;
        }
    }
}
