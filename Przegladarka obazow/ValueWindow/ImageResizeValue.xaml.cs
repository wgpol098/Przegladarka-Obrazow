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
using System.Windows.Shapes;

namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy ImageResizeValue.xaml
    /// </summary>
    public partial class ImageResizeValue : Window
    {
        int width;
        int height;
        public ImageResizeValue(Image zdj)
        {
            InitializeComponent();

            System.IO.Stream stream = new System.IO.MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)zdj.Source));
            pngEncoder.Save(stream);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);

            width = bitmap.Width;
            height = bitmap.Height;

            XPX.Text = width.ToString();
            YPX.Text = height.ToString();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            
            //Sprawdzanie czy jest podana liczba
            if (XPX.Text != "" && YPX.Text != "" && XPX.Text[0].ToString() != "0" && YPX.Text[0].ToString() != "0")
            {

                for (int i = 0; i < XPX.Text.Length; i++)
                    if (XPX.Text[i] >= '0' && XPX.Text[i] <= '9') ;
                    else
                    {
                        MessageBox.Show("Podano błędne dane!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                for (int i = 0; i < YPX.Text.Length; i++)
                    if (XPX.Text[i] >= '0' && YPX.Text[i] <= '9') ;
                    else
                    {
                        MessageBox.Show("Podano błędne dane!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                width = Convert.ToInt32(XPX.Text);
                height = Convert.ToInt32(YPX.Text);
                this.Close();
            }
            else MessageBox.Show("Podano błędne dane!","Błąd!",MessageBoxButton.OK,MessageBoxImage.Error);
        }

        private void TextChangedResolution1(object sender, RoutedEventArgs e)
        {
            if(Checkbox.IsChecked == true)
            {
                if(XPX.Text!="")
                {
                    int x = Convert.ToInt32(XPX.Text);
                    double proportion = x / (double)width;
                    int y = (int)((double)height * proportion);
                    YPX.Text = y.ToString();
                } 
            }
        }

        private void TextChangedResolution2(object sender, RoutedEventArgs e)
        {
            if (Checkbox.IsChecked == true)
            {
                if (YPX.Text != "")
                {
                    int y = Convert.ToInt32(YPX.Text);
                    double proportion = y / (double)height;
                    int x = (int)((double)width * proportion);
                    XPX.Text = x.ToString();
                }
            }
        }
        public int GetWidth() => width;
        public int GetHeight() => height;

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
