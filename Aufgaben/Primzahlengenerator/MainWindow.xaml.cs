using System.Windows;
using System.Windows.Media.Animation;
using SimpleThreadApp;

namespace Primzahlengenerator
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            int threads = int.Parse(ThreadsBox.Text);
            int prim = int.Parse(PrimBox.Text);

            Primzahlenberechner berechner = new Primzahlenberechner();

            // Warte-Animation 
            Storyboard r = (Storyboard)FindResource("loadingRotation");
            r.Begin(this, true);
            image1.Visibility = Visibility.Visible;
            
            int primzahl = await Task.Run(() => berechner.BerechnePrimzahl(threads, prim));

            // Warte-Animation beenden
            r.Stop();
            image1.Visibility = Visibility.Hidden;

            Zahlbox.Text = primzahl.ToString();
        }
    }
}