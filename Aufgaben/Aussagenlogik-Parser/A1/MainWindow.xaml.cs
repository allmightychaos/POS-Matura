using System;
using System.Collections.Generic;
using System.Windows;
using A1.Grammatik;

namespace A1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<bool> _tabelle = new List<bool>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // 1. Parser anlegen
            AbstractExpression parser = new LogicParser(); //mit konkreter Klasse anlegen

            // 2. Variablenliste zurücksetzen
            AbstractExpression.variables.Clear();

            // 3. Formel parsen
            List<char> formel = new List<char>(textBox.Text.Replace(" ", "").ToCharArray());
            parser.ParseFormel(formel);

            // 4. Auswerten für Feld 0001
            // bool result = parser.Interpret(getBooleanFromInt(AbstractExpression.variables, 1));

            int anzahlZeilen = (int)Math.Pow(2, AbstractExpression.variables.Count);
            Diagramm.VariableCount = AbstractExpression.variables.Count;

            for (int i = 0; i < anzahlZeilen; i++)
            {
                // 1. True/False Kombination holen für die Zeile
                var ctx = getBooleanFromInt(AbstractExpression.variables, i);

                // 2. in den Baum parsen
                bool ergebnis = parser.Interpret(ctx);
                Diagramm.SetValue(i, ergebnis);

                // 3. Speichern in einer Liste
                _tabelle.Add(ergebnis);
            }
        }

        public Dictionary<char, bool> getBooleanFromInt(List<char> variables, int value)
        {
            string binary = Convert.ToString(value, 2).PadLeft(variables.Count, '0');

            if (variables.Count != binary.Length)
                throw new ArgumentException("Not enough variables");

            Dictionary<char, bool> res = new Dictionary<char, bool>();

            for (int i = 0; i < variables.Count; i++)
            {
                res.Add(variables[i], binary[i] == '0' ? false : true);
            }

            return res;
        }
    }
}
