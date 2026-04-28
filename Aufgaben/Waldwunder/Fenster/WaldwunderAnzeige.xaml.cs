using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Waldwunder.Fenster
{
    /// <summary>
    /// Interaktionslogik für WaldwunderAnzeige.xaml
    /// </summary>
    public partial class WaldwunderAnzeige : Window
    {
        DataModel.Waldwunder _waldwunder;
        public WaldwunderAnzeige(DataModel.Waldwunder waldwunder, List<DataModel.Bilder> bilderListe)
        {
            InitializeComponent();

            // Daten einfügen + DataContext setzten
            _waldwunder = waldwunder;
            this.DataContext = _waldwunder;


            // Bilder einfügen
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var item in bilderListe)
            {
                var path = System.IO.Path.Combine(basePath, "Bilder", item.Name);

                Image bild = new Image
                {
                    Source = new BitmapImage(new Uri(path))
                };

                UniGrid.Children.Add(bild);
            }
        }
    }
}
