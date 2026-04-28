using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;
using KMeans.Datenstruktur;
using Microsoft.Win32;

namespace KMeans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        ObservableCollection<Person> people;
        KMeans.Algo.KMeans kmeans;
        public List<ClusterWrapper> clusters;
        public List<Brush> brushes = new List<Brush>()
        {
            Brushes.Cyan,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Magenta,
            Brushes.Yellow,
            Brushes.Orange,
            Brushes.Purple
        };

        // Statusbar
        private string _aktuellerStatus;
        public string aktuellerStatus
        {
            get { return _aktuellerStatus; }
            set
            {
                _aktuellerStatus = value;
                OnPropertyChanged("aktuellerStatus");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            people = new ObservableCollection<Person>();
            clusters = new List<ClusterWrapper>();

            this.DataContext = this;
        }


        // ======================== MENU ITEM CLICKS ======================== \\


        private void Laden_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "XML-Dateien (*.xml)|*.xml";

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                var filePath = fileDialog.FileName;

                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(People));

                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                    {
                        var container = (People)serializer.Deserialize(stream);

                        foreach (var item in container.Person)
                        {
                            people.Add(item);
                            drawCanvas(item.PixelX, item.PixelY, false, Brushes.Red);
                        }
                    }

                    aktuellerStatus = "XML geladen!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ungültiges XML-Format", "Ungültig", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML-Datei (*.xml)|*.xml";

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                var filePath = saveFileDialog.FileName;

                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(People));
                    People container = new People
                    {
                        Person = this.people.ToList()
                    };

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        serializer.Serialize(stream, container);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            aktuellerStatus = "XML gespeichert.";
        }

        private void Leeren_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
            Array.Clear(kmeans.centroid);

            aktuellerStatus = "Canvas geleert.";
            ErzeugenBtn.IsEnabled = true;
        }




        // ======================== DRAW CANVAS ======================== \\
        public void drawCanvas(double pX, double pY, bool shapes, Brush color)
        {
            // shapes:| false == ellipse // true == rectangle
            Shape shape;

            if (shapes)
            {
                // Draw rectangle
                shape = new Rectangle()
                {
                    Width = 8,
                    Height = 8,
                    Fill = color
                };
            }
            else
            {
                // Draw circle
                shape = new Ellipse()
                {
                    Width = 8,
                    Height = 8,
                    Fill = color
                };
            }


            // error handling
            if (pX <= 400 && pY <= 400)
            {
                Canvas.SetLeft(shape, pX);
                Canvas.SetTop(shape, pY);
            }
            else
            {
                MessageBox.Show("PixelX oder Y muss zwischen 0-400px sein!", "Ungültig", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Add to canvas
            MyCanvas.Children.Add(shape);
        }




        // ======================== BUTTON CLICKS ======================== \\
        private void Person_Click(object sender, RoutedEventArgs e)
        {
            // PERSON ERSTELLEN

            Person person;

            try
            {
                // Person erstellen
                string name = NameBox.Text;
                double pX = int.Parse(XBox.Text);
                double pY = int.Parse(YBox.Text);

                person = new Person
                {
                    Name = name,
                    PixelX = pX,
                    PixelY = pY
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ungültig: {ex.Message}", "Ungültig", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            people.Add(person);
            drawCanvas(person.PixelX, person.PixelY, false, Brushes.Red);

            aktuellerStatus = "Person gespeichert.";
        }

        private void Cluster_Click(object sender, RoutedEventArgs e)
        {
            // CLUSTER ERSTELLEN

            int clusterCount;

            try
            {
                clusterCount = int.Parse(KBox.Text);

                // disable "Cluster erzeugen" button 
                Button button = (Button)sender;
                button.IsEnabled = false;

                // enable "Weiter" button
                WeiterBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ungültig: {ex.Message}", "Ungültig", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ====================================== \\

            // turn ObsCollection<People> -> List<People> for kmeans
            List<Person> peoplesList = new List<Person>();
            foreach (var person in people)
            {
                peoplesList.Add(person);
            }

            // create kmeans object
            kmeans = new Algo.KMeans(peoplesList, clusterCount);

            Random random = new Random();
            kmeans.InitializeCentroids(random);

            // Add cluster to canvas
            for (int i = 0; i < kmeans.centroid.Length; i++)
            {
                // get random color for cluster
                int index = random.Next(brushes.Count());
                Brush brush = brushes[index];

                clusters.Add(new ClusterWrapper
                {
                    clusterId = i,
                    centroid = kmeans.centroid[i],
                    brushColor = brush
                });

                double pX = kmeans.centroid[i].X;
                double pY = kmeans.centroid[i].Y;

                drawCanvas(pX, pY, true, brush);
            }

            aktuellerStatus = "Cluster initialisiert. Klicke auf den \"Weiter >\"-Button.";
        }

        private void Weiter_Click(object sender, RoutedEventArgs e)
        {
            int status = kmeans.status;

            // initialized or updated centroids -> calculate distance
            if (status == 1 || status == 3)
            {
                bool result = kmeans.calcDistance();
                Render();

                aktuellerStatus = "Distanz wurde berechnet.";

                if (result)
                {
                    // kmeans converged
                    MessageBox.Show("KMeans converged!");
                    aktuellerStatus = "KMeans ist konvergiert.";
                    
                    // Button disablen
                    WeiterBtn.IsEnabled = false;
                }
            }
            // distance calculated -> update centroids
            else if (status == 2)
            {
                kmeans.updateCentroid();

                for (int i = 0; i < kmeans.centroid.Length; i++)
                {
                    ClusterWrapper cluster = clusters.FirstOrDefault(f => f.clusterId == i);
                    cluster.centroid = kmeans.centroid[i];
                }

                Render();

                aktuellerStatus = "Centroids wurden geupdated.";
            }

        }

        private void Render()
        {
            MyCanvas.Children.Clear();

            foreach (var item in clusters)
            {
                // draw cluster back on canvas
                drawCanvas(item.centroid.X, item.centroid.Y, true, item.brushColor);

                List<Person> peopleList = people.Where(w => w.clusterID == item.clusterId).ToList();

                foreach (var person in peopleList)
                {
                    drawCanvas(person.PixelX, person.PixelY, false, item.brushColor);
                }
            }
        }


        // ======================== INOTIFYPROPERTYCHANGED ======================== \\
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}