using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logika interakcji dla klasy ImageResizeValue.xaml
    /// </summary>
    public partial class ImageResizeValue : Window
    {
        private int width;
        private int height;
        private bool modified = false;
        public ImageResizeValue(Image zdj)
        {
            InitializeComponent();

            using (System.IO.Stream stream = new System.IO.MemoryStream())
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)zdj.Source));
                pngEncoder.Save(stream);

                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream))
                {
                    width = bitmap.Width;
                    height = bitmap.Height;
                    XPX.Text = width.ToString();
                    YPX.Text = height.ToString();
                    bitmap.Dispose();
                }
                stream.Close();
                stream.Dispose();
            }   
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            width = Convert.ToInt32(XPX.Text);
            height = Convert.ToInt32(YPX.Text);
            modified = true;
            Close();
        }

        private void TextChangedResolution1(object sender, RoutedEventArgs e)
        {
            if(Checkbox.IsChecked == true)
                if(XPX.Text!="")
                {
                    long x = Convert.ToInt64(XPX.Text);
                    double proportion = x / (double)width;
                    long y = (long)(height * proportion);
                    YPX.Text = y.ToString();
                } 
        }

        private void TextChangedResolution2(object sender, RoutedEventArgs e)
        {
            if (Checkbox.IsChecked == true)
                if (YPX.Text != "")
                {
                    long y = Convert.ToInt64(YPX.Text);
                    double proportion = y / (double)height;
                    long x = (long)(width * proportion);
                    XPX.Text = x.ToString();
                }
        }
        public int GetWidth() => width;
        public int GetHeight() => height;
        public bool ModifiedStatus() => modified;
        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
