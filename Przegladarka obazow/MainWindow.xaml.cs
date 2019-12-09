using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;



namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        readonly string folderFileName = "folders.txt";
        public MainWindow()
        {
            InitializeComponent();
            RefreshFolders();
            Title = "Edytor zdjęć";
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            if (d.ShowDialog() == true)
            {
                Image bit = new Image();
                bit.Source = new BitmapImage(new Uri(d.FileName));
                Edycja_zdjecia edit = new Edycja_zdjecia(bit);
                edit.Show();
            }         
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Foldery fold = new Foldery(folderFileName);
            fold.Show();
        }

        private void GoToEdition(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                if(e.ClickCount == 2)
                {
                    Edycja_zdjecia edit = new Edycja_zdjecia((Image)sender);
                    edit.Show();
                    
                    
                }
            if(e.RightButton == MouseButtonState.Pressed)
            {
                MessageBox.Show("Tutaj dodasz przeglądanie plików po kolei w takim dużym okienku");
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            thumbnails.Items.Clear();
            RefreshFolders();         
        }

        private void RefreshFolders()
        {
            if (File.Exists(folderFileName))
            {
                StreamReader file = new StreamReader(folderFileName);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (Directory.Exists(line))
                    {
                        var pliki = Directory.EnumerateFiles(line/*,"*.*", SearchOption.AllDirectories*/).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".gif"));
                        for (int i = 0; i < pliki.Count(); i++)
                            thumbnails.Items.Add(pliki.ElementAt(i));
                    }
                    else MessageBox.Show("Lokalizacja: " + line + " nie istnieje!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                file.Close();
            }
            else EmptyFolder();
        }

        private void EmptyFolder()
        {
            MessageBox.Show("Nie masz żadnych folderów na liście!\nZalecane jest dodanie przynajmniej jednego!","Ostrzeżenie!", MessageBoxButton.OK, MessageBoxImage.Warning);
            StreamWriter st = new StreamWriter(folderFileName);
            st.Close();

            Foldery fd = new Foldery(folderFileName);
            fd.ShowDialog();
            RefreshFolders();
        }

        private void CreateThumbnails(object sender, RoutedEventArgs e)
        {
            Image s = (Image)sender;
            Stream stream = new MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)s.Source));
            pngEncoder.Save(stream);
            BitmapImage bmp = new BitmapImage();

            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.DecodePixelWidth = 200;
            bmp.DecodePixelHeight = 200;
            bmp.EndInit();

            //File.Delete(stream.ToString());
            s.Source = bmp;
        }
    }
}
