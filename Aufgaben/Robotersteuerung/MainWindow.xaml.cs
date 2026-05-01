using System.IO;
using System.Windows;
using Microsoft.Win32;
using Robotersteuerung.Grammatik;

namespace Robotersteuerung
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void XMLaden_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML-Datei (*.xml)|*.xml";
            fileDialog.Multiselect = false;

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                var filePath = fileDialog.FileName;

                try
                {
                    Field.LoadField(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ungültige XML: {ex}", "Ungültige XML",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void TXTLaden_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "TXT-Datei (*.txt)|*.txt";
            fileDialog.Multiselect = false;

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                var filePath = fileDialog.FileName;

                try
                {
                    string fileContent = File.ReadAllText(filePath);

                    Parser parser = new Parser(fileContent);
                    IExpression pe = parser.ParseProgram();

                    await Task.Run(() => pe.Interpret(new Rucksack(Field)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ungültige Grammatik: {ex}", "Ungültige Grammatik",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}