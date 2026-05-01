using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AbcRobotCore;

namespace Robotersteuerung.Grammatik
{
    public class Parser
    {
        Queue<string> tokens;

        public Parser(string input)
        {
            tokens = new Queue<string>();
            tokenize(input);
        }

        private void tokenize(string input)
        {
            string[] cleaned = input.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in cleaned)
            {
                tokens.Enqueue(item);
            }
        }

        public IExpression ParseProgram()
        {
            List<IExpression> body = new List<IExpression>();

            while (tokens.Count > 0)
            {
                body.Add(ParseStatement());
            }

            return new ProgramExpression(body);
        }

        private IExpression ParseStatement()
        {
            var token = tokens.Dequeue();

            switch (token)
            {
                case "MOVE":    return ParseMove();
                case "COLLECT": return ParseCollect();
                case "IF":      return ParseIf();
                case "UNTIL":   return ParseUntil();
                case "REPEAT":  return ParseRepeat();
                default:        throw new Exception($"Unbekanntes Token: {token}");
            }
        }


        private IExpression ParseRepeat()
        {
            // "REPEAT"... has been removed

            // <ZAHL>

            if (!int.TryParse(tokens.Dequeue(), out int zahl))
            {
                throw new Exception("Erwartet Zahl nach REPEAT");
            }

            if (tokens.Peek() != "{") throw new Exception($"Erwartet: {{. Erhalten: {tokens.Peek()}");

            var body = ParseBlock();
            return new RepeatExpression(zahl, body);
        }

        private IExpression ParseUntil()
        {
            // "UNTIL"... has been removed
            var richtung = tokens.Dequeue(); // <RICHTUNG>

            // "IS-A"
            if (tokens.Peek() != "IS-A") throw new Exception($"Erwartet: IS-A. Erhalten: {tokens.Peek()}");
            tokens.Dequeue();

            var obstacle = tokens.Dequeue(); // <OBSTACLE>

            // "{"
            var body = ParseBlock();

            return new UntilExpression(body, obstacle, richtung);
        }

        private List<IExpression> ParseBlock()
        {
            List<IExpression> body = new List<IExpression>();

            // "{"
            if (tokens.Peek() != "{") throw new Exception($"Erwartet: {{. Erhalten: {tokens.Peek()}");
            tokens.Dequeue();
            
            while (tokens.Peek() != "}")
            {
                body.Add(ParseStatement());
            }

            // "}"
            tokens.Dequeue();

            return body;
        }

        private IExpression ParseIf()
        {
            // "IF"... has been removed
            var richtung = tokens.Dequeue(); // <RICHTUNG>

            // "IS-A"
            if (tokens.Peek() != "IS-A") throw new Exception($"Erwartet: IS-A. Erhalten: {tokens.Peek()}");
            tokens.Dequeue();
                
            var obstacle = tokens.Dequeue(); // <OBSTACLE>
                
            // "{"
            var body = ParseBlock();

            return new IfExpression(body, obstacle, richtung);
        }

        private IExpression ParseCollect()
        {
            return new CollectExpression();
        }

        private IExpression ParseMove()
        {
            MoveExpression move = new MoveExpression(tokens.Dequeue());
            return move;
        }
    }
}
