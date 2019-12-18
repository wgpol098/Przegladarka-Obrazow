using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy Informacje.xaml
    /// </summary>
    public partial class Informacje : Window
    {
        public Informacje(Image zdj, String imagename)
        {
            InitializeComponent();
            OkButton.Focus();

            System.IO.Stream stream = new System.IO.MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)zdj.Source));
            pngEncoder.Save(stream);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);


            var fi1 = new FileInfo(imagename);

            FileNameLabel.Content = "Nazwa pliku: " + fi1.Name;
            FilePathLabel.Content = "Ścieżka pliku: " + fi1.FullName;
            ResolutionLabel.Content = "Rozdzielczość: " + bitmap.Width + "x" + bitmap.Height;
            FormatLabel.Content = "Rozszerzenie pliku: " + fi1.Extension;

            String tmp = bitmap.PixelFormat.ToString();
            tmp = tmp.Remove(0, 6);
            PixelFormatLabel.Content = "Format pikseli: " + tmp;

            DiscSizeLabel.Content = "Rozmiar pliku : " + (float) Math.Round((double)fi1.Length/1024/1024,3) + " MB [ " + fi1.Length + " Bytes]";
            MemorySizeLabel.Content = "Rozmiar w pamięci: " + (float) Math.Round((double)stream.Length/1024/1024,3) + " MB [ " + stream.Length + " Bytes]";

            DateLabel.Content = "Data utworzenia pliku: " + File.GetCreationTime(imagename);
            LastDateModificationLabel.Content = "Data ostatniej modyfikacji pliku: " + File.GetLastWriteTime(imagename);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
