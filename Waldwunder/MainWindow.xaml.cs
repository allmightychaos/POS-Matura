using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using LinqToDB;
using Waldwunder.Fenster;

namespace Waldwunder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataModel.WaldwunderDb ctx;
        public ObservableCollection<DataModel.Waldwunder> WaldwunderListe { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            WaldwunderListe = new ObservableCollection<DataModel.Waldwunder>();

            // ==== Datenbank ==== \\
            string dbPfad = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datenbank", "Waldwunder.db");

            var options = new DataOptions()
                .UseSQLite($"Data Source={dbPfad}");

            ctx = new DataModel.WaldwunderDb(
                new DataOptions<DataModel.WaldwunderDb>(options));

            // ==== Waldwunder Laden ==== \\
            // WwunderLaden();
        }

        private void NeuesWwunder_Click(object sender, RoutedEventArgs e)
        {
            NeuesWaldwunderDialog fenster = new NeuesWaldwunderDialog();
            fenster.ShowDialog();

            // WaldwunderListe.Add(fenster.neuesWunder);
            long? id = ctx.InsertWithInt64Identity(fenster.neuesWunder);
            WwunderLaden();

            foreach (var bild in fenster.ImagesList)
            {
                bild.Wonder = id;

                ctx.InsertWithInt64Identity(bild);
            }
        }

        private void WwunderSuchen_Click(object sender, RoutedEventArgs e)
        {
            WwunderLaden();
        }

        private void WwunderLaden()
        {
            WaldwunderListe.Clear();

            var query = ctx.Waldwunders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(StichwortBox.Text))
            {
                query = query.Where(w => w.Name.Contains(StichwortBox.Text) || w.Description.Contains(StichwortBox.Text));
            }

            if (!string.IsNullOrWhiteSpace(ArtBox.Text))
            {
                query = query.Where(w => w.Type == ArtBox.Text);
            }

            if (!string.IsNullOrWhiteSpace(LatBox.Text))
            {
                decimal lat = decimal.Parse(LatBox.Text);
                decimal latFrom = lat - 0.5m;
                decimal latTo = lat + 0.5m;

                query = query.Where(w => w.Latitude >= latFrom && w.Latitude <= latTo);
            }

            if (!string.IsNullOrWhiteSpace(LongBox.Text))
            {
                decimal lon = decimal.Parse(LongBox.Text);
                decimal lonFrom = lon - 0.5m;
                decimal lonTo = lon + 0.5m;

                query = query.Where(w => w.Longitude >= lonFrom && w.Longitude <= lonTo);
            }

            var result = query.ToList();

            foreach (var item in result)
            {
                WaldwunderListe.Add(item);
                drawCanvas((decimal)item.Latitude, (decimal)item.Longitude);
            }

            /*
            foreach (var waldwunder in ctx.Waldwunders)
            {4
                WaldwunderListe.Add(waldwunder);
            }
            */
        }

        private void drawCanvas(decimal lat, decimal lon)
        {
            double pixelX = getPixel(lon, false);
            double pixelY = getPixel(lat, true);

            // Ellipse
            Ellipse ellipse = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Red,
                Cursor = Cursors.Hand
            };

            Canvas.SetLeft(ellipse, pixelX);
            Canvas.SetTop(ellipse, pixelY);

            KarteCanvas.Children.Add(ellipse);
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(KarteCanvas);
            double posX = point.X;
            double posY = point.Y;

            // Query um Long & Lat von dem Punkt zu finden mit einem Pixel-Offset von 0.5
            var query = WaldwunderListe.AsQueryable();

            // Latitude
            query = query.Where(
                w => getPixel((decimal)w.Latitude, true) >= posY - 4
                && getPixel((decimal)w.Latitude, true) <= posY + 4);

            // Longitude
            query = query.Where(
                w => getPixel((decimal)w.Longitude, false) >= posX - 4
                && getPixel((decimal)w.Longitude, false) <= posX + 4);

            // Item holen von LINQ-Query
            var result = query.FirstOrDefault();

            // Selektiere das Waldwunder in der Listbox
            WwunderListbox.SelectedItem = result;
        }

        private double getPixel(decimal value, bool reverse)
        {
            decimal maxLon = 17.231941m;
            decimal minLon = 9.362383m;
            decimal maxLat = 49.063175m;
            decimal minLat = 46.308597m;

            decimal prozent;
            double pixel;

            if (reverse)
            {
                // Latitude: (maximum - wert) / (maximum - minimum) // <= weil bei Canvas oben 0 ist statt unten (Karte)
                prozent = (maxLat - value) / (maxLat - minLat);
                pixel = (double)prozent * KarteCanvas.ActualHeight;
            }
            else
            {
                // Longitude: (wert - minimum) / (maximum - minimum)
                prozent = (value - minLon) / (maxLon - minLon);
                pixel = (double)prozent * KarteCanvas.ActualWidth;
            }

            return pixel;
        }

        private void Anzeige_Click(object sender, RoutedEventArgs e)
        {
            var selected = (DataModel.Waldwunder)WwunderListbox.SelectedItem;
            List<DataModel.Bilder> bilderliste = ctx.Bilders
                .Where(w => w.Wonder == selected.Id)
                .ToList();

            WaldwunderAnzeige anzeige = new WaldwunderAnzeige(selected, bilderliste);
            anzeige.Show();
        }
    }
}