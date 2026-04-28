using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Waldwunder.Fenster
{
    /// <summary>
    /// Interaktionslogik für NeuesWaldwunderDialog.xaml
    /// </summary>
    public partial class NeuesWaldwunderDialog : Window
    {
        private List<string> ImagePaths { get; set; }
        public ObservableCollection<DataModel.Bilder> ImagesList { get; set; }

        public DataModel.Waldwunder neuesWunder { get; set; }

        public NeuesWaldwunderDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            ImagesList = new ObservableCollection<DataModel.Bilder>();
            ImagePaths = new List<string>();
        }

        private void Registrieren_Click(object sender, RoutedEventArgs e)
        {
            decimal votes = 0;
            if (!string.IsNullOrWhiteSpace(StimmenBox.Text))
            {
                decimal.Parse(StimmenBox.Text);
            }

            // Neues Waldwunder anlegen
            neuesWunder = new DataModel.Waldwunder
            {
                Name = NameBox.Text,
                Description = BeschreibungBox.Text,
                Province = ProvinzBox.Text,
                Latitude = decimal.Parse(LatBox.Text, System.Globalization.CultureInfo.InvariantCulture),
                Longitude = decimal.Parse(LongBox.Text, System.Globalization.CultureInfo.InvariantCulture),
                Type = ArtBox.Text,
                Votes = votes
            };

            addToFolder();

            // Dialog schließen
            this.DialogResult = true;
        }

        private void BilderLaden_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Bilder (.png;.jpg;.jpeg)|*.png;*.jpg;*.jpeg|Alle Dateien (*.*)|*.*";


            if (dialog.ShowDialog() == true)
            {
                foreach (string datei in dialog.FileNames)
                {
                    ImagePaths.Add(datei);

                    DataModel.Bilder bild = new DataModel.Bilder
                    {
                        Name = Path.GetFileName(datei)
                    };

                    ImagesList.Add(bild);
                }
            }
        }

        private void BilderEntfernen_Click(object sender, RoutedEventArgs e)
        {
            var itemsToRemove = BilderListbox.SelectedItems.Cast<DataModel.Bilder>().ToList();
            foreach (DataModel.Bilder item in itemsToRemove)
            {
                ImagesList.Remove(item);
            }
        }

        private void addToFolder()
        {
            string basis = AppDomain.CurrentDomain.BaseDirectory;
            string pfad = Path.Combine(basis, "Bilder");

            if (!Directory.Exists(pfad))
            {
                Directory.CreateDirectory(pfad);
            }

            foreach (DataModel.Bilder bild in ImagesList)
            {
                int incr = 0;

                // Bspw. "IMG-123.jpeg"
                string dateiBasis = Path.GetFileNameWithoutExtension(bild.Name); // => "IMG-123"
                string dateiExtension = Path.GetExtension(bild.Name);            // => ".jpeg"

                int index = ImagesList.IndexOf(bild);   // Index in der ImagesList von dem aktuellen Bild
                string bildPfad = ImagePaths[index];    // Bildpfad (wo die Datei liegt die der Nutzer auswählt)

                string dateiPfad = Path.Combine(pfad, bild.Name); // neuer Dateipfad im `Bilder/`-Ordner

                // Bild vorhanden -> `_x` als Dateiende hinzufügen 
                while (File.Exists(dateiPfad))
                {
                    incr++;

                    string datei = dateiBasis + "_" + incr;     // => "IMG-123_1"
                    datei += dateiExtension;                    // => "IMG-123_1.jpeg"

                    dateiPfad = Path.Combine(pfad, datei);
                }

                // Bild vom alten Pfad zum neuen kopieren
                File.Copy(bildPfad, dateiPfad, false);
            }
        }

    }
}
