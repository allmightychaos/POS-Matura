using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Bildmanipulation
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

        private void Clock90_Click(object sender, RoutedEventArgs e)
        {
            TransformedBitmap bitmap = rotate(90);
            // Bild rotieren
            Racoon.Source = bitmap;

            // zu Frame
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // zu file
            using (FileStream fs = new FileStream("C:\\Users\\wegho\\Downloads\\POS-Matura\\Aufgaben\\Bildmanipulation\\racoon_swimming.jpg", FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

        private void Counter90_Click(object sender, RoutedEventArgs e)
        {
            TransformedBitmap bitmap = rotate(-90);
            // Bild rotieren
            Racoon.Source = bitmap;

            // zu Frame
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // zu file
            using (FileStream fs = new FileStream("C:\\Users\\wegho\\Downloads\\POS-Matura\\Aufgaben\\Bildmanipulation\\racoon_swimming.jpg", FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

        private void Rotate180_Click(object sender, RoutedEventArgs e)
        {
            TransformedBitmap bitmap = rotate(180);
            // Bild rotieren
            Racoon.Source = bitmap;

            // zu Frame
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // zu file
            using (FileStream fs = new FileStream("C:\\Users\\wegho\\Downloads\\POS-Matura\\Aufgaben\\Bildmanipulation\\racoon_swimming.jpg", FileMode.Create))
            {
                encoder.Save(fs);
            }
        }


        private TransformedBitmap rotate(int angle)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri("C:\\Users\\wegho\\Downloads\\POS-Matura\\Aufgaben\\Bildmanipulation\\racoon_swimming.jpg");
            bitmap.EndInit();
            bitmap.Freeze();

            TransformedBitmap transformed = new TransformedBitmap();
            transformed.BeginInit();
            transformed.Source = bitmap;
            transformed.Transform = new RotateTransform(angle);
            transformed.EndInit();
            transformed.Freeze();

            return transformed;
        }



        private void String_Click(object sender, RoutedEventArgs e)
        {
            // Ordner zum speichern holen
            string path = getPath();

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames
                .Add(BitmapFrame
                    .Create
                        (new Uri("C:\\Users\\wegho\\Downloads\\POS-Matura\\Aufgaben\\Bildmanipulation\\racoon_swimming.jpg")));


            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                byte[] imageBytes = ms.ToArray();

                // String erstellen:
                string imageString = Convert.ToBase64String(imageBytes);

                File.WriteAllText(path, imageString);
            }
        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            // Ordner zum speichern holen
            string path = getPath();

            // bild zu string konvertieren
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(new Uri("C:\\Users\\wegho\\Downloads\\POS-Matura\\Aufgaben\\Bildmanipulation\\racoon_swimming.jpg")));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }



        private string getPath()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Alle (*.*)|*.*";

            bool? result = saveFileDialog.ShowDialog();

            string path = "";
            if (result == true)
            {
                path = saveFileDialog.FileName;
            }

            return path;
        }
    }
}