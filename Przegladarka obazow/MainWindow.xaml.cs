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
using System.Text.RegularExpressions;
using Accord.Imaging.Filters;

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
                string tmpEdition = Regex.Replace(file.ReadToEnd(), @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);
                file.Close();

                StreamWriter zapis = new StreamWriter(folderFileName);
                zapis.Write(tmpEdition);
                zapis.Close();

                file = new StreamReader(folderFileName);
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (Directory.Exists(line))
                    {
                        var pliki = Directory.EnumerateFiles(line/*,"*.*", SearchOption.AllDirectories*/).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".gif") || s.EndsWith(".JPG") || s.EndsWith(".BMP") || s.EndsWith(".PNG") || s.EndsWith(".GIF"));
                        for (int i = 0; i < pliki.Count(); i++)
                            thumbnails.Items.Add(pliki.ElementAt(i));
                    }
                    else
                    {
                        var result = MessageBox.Show("Lokalizacja: " + line + " nie istnieje!\nUsunąć z listy?", "Error!", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if(result == MessageBoxResult.Yes)
                        {
                            int last = tmpEdition.IndexOf(line) + line.Length;
                            int first = tmpEdition.LastIndexOf(line);
                            tmpEdition = tmpEdition.Remove(first, last - first);
                        }
                    } 
                } 
                file.Close();

                zapis = new StreamWriter(folderFileName);
                zapis.Write(Regex.Replace(tmpEdition, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline));
                zapis.Close();
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

        private void CreateThumbnails(object sender)
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
                        pen.Dispose();
                    }
                    graphics.Dispose();
                }
            }
            tmp.Dispose();
            grayImage.Dispose();
            GC.Collect(0, GCCollectionMode.Forced);
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

        private void Test_wydajnosci(object sender, RoutedEventArgs e)
        {
            StreamWriter test = new StreamWriter("test.txt");

            //test.WriteLine("Negatyw z biblioteki" + TimeFunction(new Invert()).ToString() + "ms");
            test.WriteLine("Mój negatyw: " + TimeFunction(new Tools.Filters.Negative(0)).ToString() + " ms");
            //test.WriteLine("Mój negatyw B: " + TimeFunction(new Tools.Filters.Negative(3)).ToString() + " ms");
            //test.WriteLine("Binarization: " + TimeFunction(new Tools.Filters.Binarization(155, 344)).ToString() + " ms");
            //test.WriteLine("HistogramAlignment: " + TimeFunction(new Tools.Filters.HistogramAlignment()).ToString() + " ms");

            test.Close();
        }

        private double TimeFunction(dynamic filter)
        {
            double ile = 0;
            double TotalTime = 0;
            string directory = "C:\\Users\\patryk\\OneDrive\\Studia\\III rok\\Programowanie w środowisku Windows\\Labolatoria\\Projekt WPF\\Przegladarka obazow\\Testy";
            if (Directory.Exists(directory))
            {
                var pliki = Directory.EnumerateFiles(directory/*,"*.*", SearchOption.AllDirectories*/).Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".gif") || s.EndsWith(".JPG") || s.EndsWith(".BMP") || s.EndsWith(".PNG") || s.EndsWith(".GIF"));
                
                for (int i = 0; i < pliki.Count(); i++)
                {
                    using (Bitmap tmp = new Bitmap(pliki.ElementAt(i)))
                    {
                        System.Diagnostics.Stopwatch time = new System.Diagnostics.Stopwatch();
                        time.Start();
                        try
                        {
                            filter.ApplyInPlace(tmp);
                            ile++;
                        }
                        catch { }
                        time.Stop();
                        TimeSpan ts = time.Elapsed;
                        TotalTime += ts.TotalMilliseconds;
                    }
                }
            }
            return TotalTime/ile;
        }
    }
}
