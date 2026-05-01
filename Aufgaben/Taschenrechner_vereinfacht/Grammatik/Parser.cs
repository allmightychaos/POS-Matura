using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Taschenrechner_vereinfacht.Grammatik
{
    public class Parser
    {
        public Queue<string> tokens;

        public Parser(string input)
        {
            tokens = new Queue<string>();
            TokeniseInput(input);
        }

        private void TokeniseInput(string input)
        {
            List<string> tokenized = new List<string>();
            List<int> errorIndices = new List<int>();
            
            string pattern = @"\d+(\.\d+)?|[a-z]+|[\+\-\*\/]+|\s+";

            Regex rx = new Regex(pattern);

            int cursor = 0;

            foreach (Match m in rx.Matches(input))
            {
                // gap
                int gap = m.Index - cursor;
                if (gap > 0)
                {
                    for (int i = cursor ; i < m.Index; i++)
                    {
                        errorIndices.Add(i);
                    }
                }

                // passt alles 
                cursor = m.Index + m.Length;

                if (!string.IsNullOrWhiteSpace(m.Value))
                {
                    tokenized.Add(m.Value);
                }
            }

            for (int i = cursor; i < input.Length; i++)
            {
                errorIndices.Add(i);
            }


            if (errorIndices.Count > 0)
            {
                throw new Exception($"Error: {string.Join(",", errorIndices)}");
                // besseres handling später
            }

            foreach (var token in tokenized)
            {
                // MessageBox.Show(token);
                tokens.Enqueue(token);
            }
        }

        // ===================================================================================== \\

        public double Calculate()
        {
            IExpression tree = ParseProgram();
            return tree.Interpret();
        }

        // ===================================================================================== \\

        private IExpression ParseWert()
        {
            if (tokens.Count == 0) throw new Exception("Unerwartetes Ende des Ausdrucks");

            // MessageBox.Show($"{tokens.Peek()}");
            if (tokens.Peek() == "(")
            {
                tokens.Dequeue();

                IExpression expr = ParseAusdruck();

                if (tokens.Dequeue() != ")") throw new Exception("Erwartet: )");
                return expr;
            }
            else if (double.TryParse(tokens.Peek(), out double num))
            {
                tokens.Dequeue();
                return new NumberExpression(num);
            }
            else
            {
                throw new Exception("Variable ist noch nicht implementiert...");
            }
        }

        private IExpression ParseFaktor()
        {
            bool negative = false;

            if (tokens.Count > 0 && tokens.Peek() == "-")
            {
                tokens.Dequeue();
                negative = true;
            }

            IExpression wert = ParseWert();
            return negative ? new UnaryExpression(wert) : wert;
        }

        private IExpression ParsePotenz()
        {
            IExpression left = ParseFaktor();
            if (tokens.Count == 0) return left;

            if (tokens.Peek() == "^")
            {
                tokens.Dequeue();
                IExpression right = ParseAusdruck();

                return new BinaryExpression(left, right, "^");
            }

            return left;
        }

        private IExpression ParseTerm()
        {
            IExpression left = ParsePotenz();
            if (tokens.Count == 0) return left;

            if (tokens.Peek() == "*")
            {
                tokens.Dequeue();
                IExpression right = ParseAusdruck();

                return new BinaryExpression(left, right, "*");
            }
            if (tokens.Peek() == "/")
            {
                tokens.Dequeue();
                IExpression right = ParseAusdruck();

                return new BinaryExpression(left, right, "/");
            }

            return left;
        }

        private IExpression ParseAusdruck()
        {
            IExpression left = ParseTerm();
            if (tokens.Count == 0) return left;

            if (tokens.Peek() == "+")
            {
                tokens.Dequeue();
                IExpression right = ParseAusdruck();

                return new BinaryExpression(left, right, "+");
            }
            if (tokens.Peek() == "-")
            {
                tokens.Dequeue();
                IExpression right = ParseAusdruck();

                return new BinaryExpression(left, right, "-");
            }

            return left;
        }

        private IExpression ParseProgram()
        {
            IExpression expression = ParseAusdruck();

            return expression;
        }

    }
}
