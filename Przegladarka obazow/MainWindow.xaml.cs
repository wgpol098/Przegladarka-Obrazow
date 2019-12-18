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
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using Przegladarka_obazow.AppWindow;

namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Do rozpoznawania twarzy
        static readonly CascadeClassifier cascadeClasifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        readonly string folderFileName = "folders.txt";
        public MainWindow()
        {
            InitializeComponent();
            RefreshFolders();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            if (d.ShowDialog() == true)
            {
                System.Windows.Controls.Image bit = new System.Windows.Controls.Image();
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
            fold.ShowDialog();
            MenuItem_Click_3(sender,e);
        }

        private void GoToEdition(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                if(e.ClickCount == 2)
                {
                    Edycja_zdjecia edit = new Edycja_zdjecia((System.Windows.Controls.Image)sender);
                    edit.Show();       
                }
            if(e.RightButton == MouseButtonState.Pressed)
            {          
                Slideshow slide = new Slideshow(thumbnails.Items,(System.Windows.Controls.Image)sender);
                slide.ShowDialog();
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
                        var pliki = Directory.EnumerateFiles(line/*,"*.*", SearchOption.AllDirectories*/).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".gif") || s.EndsWith(".JPG") || s.EndsWith(".BMP") || s.EndsWith(".PNG") || s.EndsWith(".GIF"));
                        for (int i = 0; i < pliki.Count(); i++)
                            thumbnails.Items.Add(pliki.ElementAt(i));

                    }
                    else MessageBox.Show("Lokalizacja: " + line + " nie istnieje!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                file.Close();
            }
            else EmptyFolder();

            if (Face.IsChecked == true ) MainFaceDetection(thumbnails.Items.Count);
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
            System.Windows.Controls.Image s = (System.Windows.Controls.Image)sender;
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

        //Zdjęcia na których są twarze.
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (Face.IsChecked == false)
            {
                this.Cursor = Cursors.Wait;
                Face.IsChecked = true;
                MainFaceDetection(thumbnails.Items.Count);
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                Face.IsChecked = false;
                thumbnails.Items.Clear();
                RefreshFolders();
            } 
        }

        private void MainFaceDetection(Int64 count)
        {

            string[] tmp;
            bool[] tmpp;
            tmp = new string[count];
            tmpp = new bool[count];

            for (int i = 0; i < count; i++)
            {
                String d = (String)thumbnails.Items.GetItemAt(i);
                if (FaceDetection(d) == true)
                {
                    tmpp[i] = true;
                    tmp[i] = d;
                }
                else tmpp[i] = false;
            }

            
            for (int i = 0; i < count; i++)
            {
                thumbnails.Items.RemoveAt(0);
            }

            thumbnails.Items.Clear();

            for (int i=0;i<count;i++)
            {
                if (tmpp[i] == true) thumbnails.Items.Add(tmp[i]);
            }
        }

        private bool FaceDetection(String filename)
        {
            Bitmap tmp = new Bitmap(filename);
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(tmp);
            System.Drawing.Rectangle[] rectangles = cascadeClasifier.DetectMultiScale(grayImage, 1.2, 1);

            int i = 0;
            foreach (System.Drawing.Rectangle rectange in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(tmp))
                {
                    using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 1))
                    {
                        graphics.DrawRectangle(pen, rectange);
                        i++;
                    }
                }
            }
            tmp.Dispose();
            tmp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (i != 0) return true;

            return false;
        }

        private void myControl_KeyDown(object sender, KeyEventArgs e)
        {
            RoutedEventArgs tmp = new RoutedEventArgs();

            //Otwórz
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
                MenuItem_Click(sender, tmp);

            //Zamknij
            if (e.Key == Key.Escape)
                MenuItem_Click_1(sender, tmp);
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            Slideshow slide = new Slideshow(thumbnails.Items);
            slide.ShowDialog();
        }
    }
}
