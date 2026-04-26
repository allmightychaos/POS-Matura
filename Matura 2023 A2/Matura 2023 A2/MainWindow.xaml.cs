using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using Kakuro;
using Matura_2023_A2.Kakuro;
using Microsoft.Win32;

namespace Matura_2023_A2
{

    public partial class MainWindow : Window
    {
        SpielerFeld prevFeld;

        public MainWindow()
        {
            InitializeComponent();
            KakuroControl kc = new KakuroControl();
        }

        // XML-Laden
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML-Dateien (*.xml)|*.xml|Alle Dateien (*.*)|*.*";
            fileDialog.Multiselect = false;

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(KakuroData));

                    using (FileStream fs = new FileStream(fileDialog.FileName, FileMode.Open))
                    {
                        KakuroData data = (KakuroData)serializer.Deserialize(fs);

                        UniGrid.Columns = data.Cols;
                        UniGrid.Rows = data.Rows;

                        for (int i = 0; i < data.Rows; i++)
                        {
                            for (int j = 0; j < data.Cols; j++)
                            {
                                UIElement feld = null;

                                int posX = j;
                                int posY = i;

                                // 1. Inaktives Feld
                                if (data.InaktiveFields.Any(w => w.X == posX && w.Y == posY))
                                {
                                    TextBlock textBlock = new TextBlock()
                                    {
                                        Background = Brushes.Black
                                    };

                                    feld = textBlock;
                                }
                                // 2. Summe-Feld
                                else if (data.Sums.Any(w => w.X == posX && w.Y == posY))
                                {
                                    Sums sum = data.Sums.FirstOrDefault(w => w.X == posX && w.Y == posY);

                                    KakuroControl kakuro = new KakuroControl()
                                    {
                                        Horizontal = sum.Horizontal,
                                        Vertical = sum.Vertical,
                                    };

                                    feld = kakuro;
                                }
                                // 3. Eingabe-Feld
                                else
                                {
                                    TextBox textBox = new TextBox()
                                    {
                                        BorderBrush = Brushes.Black,
                                        FontSize = 20,
                                        HorizontalContentAlignment = HorizontalAlignment.Center,
                                        VerticalContentAlignment = VerticalAlignment.Center,
                                        Tag = new SpielerFeld(posX, posY)
                                    };
                                    textBox.LostFocus += Eingabe_LostFocus;

                                    feld = textBox;
                                }

                                UniGrid.Children.Add(feld);
                            }
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show(
                        "Ungültige XML!",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                        );
                    return;
                }
            }
        }

        // User-Eingabe
        private void Eingabe_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBlock = (TextBox)sender;
            string text = textBlock.Text;

            SpielerFeld feld = (SpielerFeld)textBlock.Tag;


            if (int.TryParse(text, out int num))
            {
                if (num < 1 || num > 9)
                {
                    textBlock.Text = feld.num.ToString();
                    MessageBox.Show(
                        "Ungültige Eingabe - Zahl muss zwischen 1 und 9 liegen.",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
                else
                {
                    if (checkRules(feld, true) && checkRules(feld, false))
                    {
                        feld.num = num;
                    }
                }
            }
        }

        /* Regeln: 
         * 1. Bei inkorrekter Kreuzsumme -> Eingabe-Felder (betroffene) Brushes.Salmon färben
         * 2. Wenn in einer Summe mehrmals dieselbe Ziffer vorkommt -> Brushes.Salmon auf die jeweiligen Felder
         * 
         * Zuerst alle Eingaben auf weiß setzten, dann Summen prüfen und ggf. färben. 
         * Kreuzsummen mit leeren Felkdern und zu kleinen Summen nicht einfärben.
        */

        private bool checkRules(SpielerFeld feld, bool orientation)
        {
            // starting positions & index
            int startPosX = feld.X;
            int startPosY = feld.Y;

            // Dict<index, number> and sum for that part
            Dictionary<int, int> indexNum = new Dictionary<int, int>();
            int sum = 0;

            // =================================================== \\

            if (orientation)
            {
                // <- links horizontal
                for (int i = startPosX; i >= 0; i--)
                {
                    if (!processFields(i, true, true))
                    {
                        break;
                    }
                }

                // horizontal rechts -> 
                for (int i = startPosX + 1; i < UniGrid.Columns; i++)
                {
                    if (!processFields(i, false, true))
                    {
                        break;
                    }
                }
            }
            else
            {
                // ↑ vertikal oben
                for (int i = startPosY; i >= 0; i--)
                {
                    if (!processFields(i, true, false))
                    {
                        break;
                    }
                }

                // ↓ vertikal unten
                for (int i = startPosY + 1; i < UniGrid.Rows; i++)
                {
                    if (!processFields(i, false, false))
                    {
                        break;
                    }
                }
            }

            // =================================================== \\

            bool processFields(int i, bool addSum, bool orientation)
            {
                // Get current index and with that index the field
                int index = orientation ? getFieldIndex(i, startPosY) : getFieldIndex(startPosX, i);
                var field = UniGrid.Children[index];


                // Case-1: if field is Textbox (userinput) 
                if (field.GetType() == typeof(TextBox))
                {
                    TextBox box = (TextBox)field;
                    // reset bg-color
                    box.Background = Brushes.White;

                    string text = box.Text;

                    // add it to the Dictionary
                    if (int.TryParse(text, out int num))
                    {
                        indexNum[index] = num;
                    }

                    return true;
                }

                // Case-2: if field is KakuroControl (= Sum)
                else if (field.GetType() == typeof(KakuroControl))
                {
                    if (addSum)
                    {
                        KakuroControl kakuro = (KakuroControl)field;

                        if (orientation) // horizontal
                        {
                            sum = (int)kakuro.Horizontal; // <- use the sum 
                        }
                        else // vertical
                        {
                            sum = (int)kakuro.Vertical;
                        }
                    }

                    return false;
                }

                // Case-3: if field is inactive
                else if (field.GetType() == typeof(TextBlock))
                {
                    return false;
                }

                return true; // keep going (empty fields)
            }

            // =================================================== \\

            int gesamtSum = indexNum.Values.Sum();
            if (gesamtSum > sum)
            {
                foreach (var index in indexNum.Keys)
                {
                    TextBox box = (TextBox)UniGrid.Children[index];
                    box.Background = Brushes.Salmon;
                }

                return false;
            }

            var duplicates = indexNum
                .GroupBy(kv => kv.Value)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Select(kv => kv.Key));

            if (duplicates.Any())
            {
                foreach (var index in duplicates)
                {
                    TextBox box = (TextBox)UniGrid.Children[index];
                    box.Background = Brushes.Salmon;
                }

                return false;
            }

            return true;
        }

        private int getFieldIndex(int x, int y)
        {
            // X(3), Y(2) -> 24 -> UniGrid[24]
            return (y * UniGrid.Rows) + x;
        }
    }
}
