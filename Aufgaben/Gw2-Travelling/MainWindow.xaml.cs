using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gw2_Travelling.Algo;

namespace Gw2_Travelling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool mouseMove = true;

        private double _posX;
        public double posX
        {
            get { return _posX;  }
            set
            {
                _posX = value;
                OnPropertyChanged("posX");
            }
        }

        private double _posY;
        public double posY
        {
            get { return _posY; }
            set
            {
                _posY = value;
                OnPropertyChanged("posY");
            }
        }

        private int _wpCount;
        public int wpCount
        {
            get { return _wpCount; }
            set
            {
                _wpCount = value;
                OnPropertyChanged("wpCount");
            }
        }

        // ----------------------------------------------------------------------------- \\


        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
        }


        // ----------------------------------------------------------------------------- \\


        private void drawCircle()
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = 12,
                Height = 12,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black
            };

            ellipse.MouseEnter += Ellipse_MouseEnter;
            ellipse.MouseLeave += Ellipse_MouseLeave;

            Canvas.SetTop(ellipse, posY - (ellipse.Height / 2));
            Canvas.SetLeft(ellipse, posX - (ellipse.Width / 2));

            Gw2Canvas.Children.Add(ellipse);
        }


        private void drawLine(Point from, Point to)
        {
            Line line = new Line()
            {
                X1 = from.X,
                X2 = to.X,
                Y1 = from.Y,
                Y2 = to.Y,

                Stroke = Brushes.Black,
                StrokeThickness = 3
            };

            Gw2Canvas.Children.Add(line);
        }





        // ----------------------------------------------------------------------------- \\
        // Click Handler

        private async void Copy_Click(object sender, RoutedEventArgs e)
        {
            string text = $"{posX}, {posY}"; // X: 50 Y:20 -> "50, 20"

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Clipboard.SetDataObject(text, true);
                    return;
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    await Task.Delay(20);
                }
            }
        }

        private void Gw2Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 1. Mausposition (X/Y) holen
            var pos = e.GetPosition((IInputElement)sender);
            
            // 2. Globale posX & posY setzten
            posX = Math.Round(pos.X, 0);
            posY = Math.Round(pos.Y, 0);

            // 3. Neuen Punkt (Wegmarke) zeichnen und zum Counter hinzu
            drawCircle();
            wpCount++;


            // Optionale:

            // Kopieren Button aktivieren
            copyBtn.IsEnabled = true;

            // mouseMove boolean umschalten
            if (mouseMove) mouseMove = false;
            else mouseMove = true;

            // wenn Wegmarken-Counter 3 oder höher ist, Algorithmen-Button aktivieren
            if (wpCount > 2)
            {
                SalesBtn.IsEnabled = true;
            }
        }

        private void Gw2Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseMove)
            {
                var pos = e.GetPosition((IInputElement)sender);

                posX = Math.Round(pos.X, 0);
                posY = Math.Round(pos.Y, 0);
            }
        }


        // ----------------------------------------------------------------------------- \\
        private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }


        // ----------------------------------------------------------------------------- \\
        private void Travelling_Click(object sender, RoutedEventArgs e)
        {
            // Liste aus Punkten für Travelling Salesman
            List<Point> points = new List<Point>();

            foreach (var wp in Gw2Canvas.Children)
            {
                // Alle Ellipsen vom Canvas holen & der Liste hinzufügen
                if (wp is Ellipse ellipse)
                {
                    double pX = Canvas.GetLeft(ellipse) + (ellipse.Width / 2);
                    double pY = Canvas.GetTop(ellipse) + (ellipse.Height / 2);

                    points.Add(new Point(pX, pY));
                }
            }

            // === Salesman === \\

            TravellingSalesman salesman = new TravellingSalesman(points);
            List<Point> path = salesman.Run();

            // Linien am Canvas zeichnen
            for (int i = 0; i < path.Count - 1; i++)
            {
                drawLine(path[i], path[i+1]);
            }
        }

        private void Djikstra_Click(object sender, RoutedEventArgs e)
        {

        }

        private void A_star_Click(object sender, RoutedEventArgs e)
        {

        }




        // ----------------------------------------------------------------------------- \\
        // Ereignis der Schnittstelle 'INotifyPropertyChanged'    

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}