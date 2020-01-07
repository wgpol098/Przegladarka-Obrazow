using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Interop;


namespace Przegladarka_obazow.AppWindow
{
    /// <summary>
    /// Logika interakcji dla klasy Slideshow.xaml
    /// </summary>
    public partial class Slideshow : Window
    {
        int ActiveImageNumber = 0;
        ItemCollection ItemsImage;
        int max = 0;
        public Slideshow(ItemCollection Items)
        {
            InitializeComponent();

            max = Items.Count;
            ItemsImage = Items;
            UpdateImage();
        }
        public Slideshow(ItemCollection Items, System.Windows.Controls.Image img)
        {
            InitializeComponent();

            //Sprawdzanie, które zdjęcie jest aktualne
            ImageSlide.Source = img.Source;
            string filename = img.Source.ToString().Remove(0, 8);
            filename = filename.Replace('/','\\');
            int count = Items.Count;
            for(int i=0;i<count;i++)
            {
                if (filename == Items.GetItemAt(i).ToString()) ActiveImageNumber = i;
            }

            max = count;
            ItemsImage = Items;
        }

        private void ClickBack(object sender, RoutedEventArgs e)
        {
            ActiveImageNumber--;
            UpdateImage();
        }

        private void ClickNext(object sender, RoutedEventArgs e)
        {
            ActiveImageNumber++;
            UpdateImage();
        }
        private void myControl_KeyDown(object sender,KeyEventArgs e)
        {
            if(e.Key == Key.Left || e.Key == Key.Down)
            {
                ActiveImageNumber--;
                UpdateImage();      
            }
            if(e.Key == Key.Right || e.Key == Key.Up)
            {
                ActiveImageNumber++;
                UpdateImage();
            }
            if(e.Key == Key.Escape)
            {
                Close();
            }
            if(e.Key == Key.Enter)
            {
                PopupAlert.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateImage()
        {
            if (ActiveImageNumber == -1) ActiveImageNumber = max-1;
            if (ActiveImageNumber == max) ActiveImageNumber = 0;
            using (Bitmap tmp = new Bitmap(ItemsImage.GetItemAt(ActiveImageNumber).ToString()))
            {
                ImageSlide.Source = null;
                ImageSlide.Source = ImageSourceFromBitmap(tmp);
                tmp.Dispose();
                GC.Collect(0, GCCollectionMode.Forced);
            }
        }

        private ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            IntPtr hBitmap = bmp.GetHbitmap();
            try
            {
                var source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return source;
            }
            finally
            {
                DeleteObject(hBitmap);
                GC.Collect(0, GCCollectionMode.Forced);
            }
        }

        private void SlideshowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ImageSlide.Source = null;
            GC.Collect();
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
