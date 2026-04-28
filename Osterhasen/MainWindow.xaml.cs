using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using LinqToDB;
using LinqToDB.Data;
using Osterhasen.Algorithm;

namespace Osterhasen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VerbindungDb ctx;

        public MainWindow()
        {
            InitializeComponent();
            var options = new DataOptions()
                .UseSQLite("Data Source=osterhasen.db");

            ctx = new VerbindungDb(
                new DataOptions<VerbindungDb>(options));

            try
            {
                ctx.CreateTable<Person>();
            }
            catch (Exception)
            {
                return;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get data from Textboxes
                string Name = NameBox.Text;
                double lng = double.Parse(LngBox.Text, CultureInfo.InvariantCulture);
                double lat = double.Parse(LatBox.Text, CultureInfo.InvariantCulture);

                // Create Person
                Person person = new Person()
                {
                    name = Name,
                    lng = lng,
                    lat = lat
                };

                // Insert to DB
                await Task.Run(() =>
                {
                    ctx.Insert(person);
                });

                Status.Visibility = Visibility.Visible;
                // Add Person as a point to map
                drawCanvas(lng, lat);
            }
            catch (Exception ex)
            {
                Status.Content = $"Person konnte nicht hinzugefügt werden.\n{ex}";
                Status.Foreground = Brushes.Red;
                Status.Visibility = Visibility.Visible;
            }
        }




        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int clusters = int.Parse(HelferBox.Text);

            K_Means kmeans = new K_Means(persons(), clusters);

            // Draw Clusters on Canvas
            Random random = new Random();
            foreach (var cluster in kmeans.centroids)
            {
                double pX = cluster.X;
                double pY = cluster.Y;

                Color randColor = Color.FromArgb(
                    255,
                    (byte)random.Next(256), 
                    (byte)random.Next(256), 
                    (byte)random.Next(256));

                SolidColorBrush color = new SolidColorBrush(randColor);

                drawRect(pX, pY, color);
            }
        }

        private List<Person> persons()
        {   
            List<Person> persons = new List<Person>();

            foreach (var person in ctx.Person)
            {
                persons.Add(person);
            }

            return persons;
        }

        // ============================= Pixels ============================= \\
        private double getPixel(double coordinate, bool direction)
        {
            // direction true => long // false => lat

            /*
            Links   16.209652   xMin    long
            Unten   47.786898   yMin    lat
            Rechts  16.281017   xMax    long
            Oben    47.846533   yMax    lat
            */

            double xMin = 16.209652;
            double xMax = 16.281017;
            double yMin = 47.786898;
            double yMax = 47.846533;

            /*
            prozentX    = (wert - xMin) / (xMax - xMin)
            pixelX      = prozentX * actualWidth

            prozentY    = (yMax - wert) / (yMax - yMin)
            pixelY      = prozentY * actualHeight

            */

            double prozent;
            double pixel;

            if (direction)
            {
                // Longitude
                prozent = (coordinate - xMin) / (xMax - xMin);
                pixel = prozent * KarteCanvas.ActualWidth;
            }
            else
            {
                // Latitude
                prozent = (yMax - coordinate) / (yMax - yMin);
                pixel = prozent * KarteCanvas.ActualHeight;
            }

            return pixel;
        }


        // ============================= Drawing ============================= \\
        private void drawCanvas(double x, double y)
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = 12,
                Height = 12,
                Fill = Brushes.Blue,
            };

            Canvas.SetLeft(ellipse, getPixel(x, true));
            Canvas.SetTop(ellipse, getPixel(y, false));

            KarteCanvas.Children.Add(ellipse);
        }


        private void drawRect(double x, double y, Brush color)
        {
            Rectangle rectangle = new Rectangle()
            {
                Width = 12,
                Height = 12,
                Fill = color
            };

            Canvas.SetLeft(rectangle, getPixel(x, true));
            Canvas.SetTop(rectangle, getPixel(y, false));

            KarteCanvas.Children.Add(rectangle);
        } 
    }
}