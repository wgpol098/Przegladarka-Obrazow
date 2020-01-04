using Accord.Imaging.Filters;
using Przegladarka_obazow.Tools.Filters;
using Przegladarka_obazow.Tools.Histogram;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Przegladarka_obazow.AppWindow
{
    /// <summary>
    /// Logika interakcji dla klasy FastFilter.xaml
    /// </summary>
    public partial class FastFilter : Window
    {
        private Bitmap org;
        private int numberfilter;
        public FastFilter(Bitmap bitmap)
        {
            InitializeComponent();

            numberfilter = 0;
            org = new Bitmap(bitmap);
            ImageFilterBox.Source = ImageSourceFromBitmap(org);
            Title = "Original";
        }

        private void KeyDownUI(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Right) numberfilter++;

            if(e.Key == Key.Left) numberfilter--;

            UpdateImage();
        }

        private void UpdateImage()
        {
            if (numberfilter == -1) numberfilter = 26;
            if (numberfilter == 0)
            {
                ImageFilterBox.Source = ImageSourceFromBitmap(org);
                Title = "Original";
            } 
            if (numberfilter == 1)
            {
                FilterAdd(new Negative(0));
                Title = "Negative ALL";
            }
            if (numberfilter == 2)
            {
                FilterAdd(new Negative(1));
                Title = "Negative R";
            }
            if (numberfilter == 3)
            {
                FilterAdd(new Negative(2));
                Title = "Negative G";
            }
            if (numberfilter == 4)
            {
                FilterAdd(new Negative(3));
                Title = "Negative B";
            }
            if (numberfilter == 5)
            {
                FilterAdd(new HistogramAlignment());
                Title = "Histogram Alignment";
            }
            if (numberfilter == 6)
            {
                FilterAdd(new HistogramStretching());
                Title = "Histogram Stretching";
            }
            if (numberfilter == 7)
            {
                FilterAdd(new GrayScale());
                Title = "GrayScale";
            }
            if (numberfilter == 8)
            {
                FilterAdd(new Tools.Filters.RotateChannels());
                Title = "Rotate Channels 1";
            } 
            if(numberfilter==9)
            {
                Cursor = Cursors.Wait;
                Tools.Filters.RotateChannels filter = new Tools.Filters.RotateChannels();
                using (Bitmap filterbmp = (Bitmap)org.Clone())
                {
                    filter.ApplyInPlace(filterbmp);
                    filter.ApplyInPlace(filterbmp);
                    ImageFilterBox.Source = ImageSourceFromBitmap((Bitmap)filterbmp);
                    GC.Collect();
                }
                Title = "Rotate Channels 2";
                Cursor = Cursors.Arrow;
            }
            if (numberfilter == 10)
            {
                FilterAdd(new Sepia());
                Title = "Sepia";
            }
            if (numberfilter == 11)
            {
                FilterAdd(new OilPainting());
                Title = "Oil Painting";
            }
            if (numberfilter == 12)
            {
                FilterAdd(new Median());
                Title = "Median Filter";
            }
            if (numberfilter == 13)
            {
                FilterAdd(new Mean());
                Title = "Mean Filter";
            }
            if (numberfilter == 14)
            {
                FilterAdd(new Jitter());
                Title = "Jitter Filter";
            }
            if (numberfilter == 15)
            {
                FilterAdd(new Exponential());
                Title = "Exponential Filter";
            }
            if (numberfilter == 16)
            {
                FilterAdd(new EuclideanColorFiltering());
                Title = "Euclidean Color Filtering";
            }
            if (numberfilter == 17)
            {
                FilterAdd(new Sharpen());
                Title = "Sharpen";
            }
            if (numberfilter == 18)
            {
                FilterAdd(new SaltAndPepperNoise());
                Title = "Salt and Pepper Noise";
            }
            if (numberfilter == 19)
            {
                FilterAdd(new RGChromacity());
                Title = "RGChromacity";
            }
            if (numberfilter == 20)
            {
                FilterAdd(new Logarithm());
                Title = "Logarithm Filter";
            }
            if (numberfilter == 21)
            {
                FilterAdd(new BilateralSmoothing());
                Title = "Bilateral Smoothing";
            }
            if (numberfilter == 22)
            {
                FilterAdd(new Blur());
                Title = "Blur Filter";
            }
            if (numberfilter == 23)
            {
                FilterAdd(new BrightnessCorrection());
                Title = "Brrightness Correction";
            }
            if (numberfilter == 24)
            {
                FilterAdd(new ConservativeSmoothing());
                Title = "Conservative Smoothing";
            }
            if (numberfilter == 25)
            {
                FilterAdd(new Edges());
                Title = "Edges Filter";
            }
            if (numberfilter == 26)
            {
                FilterAdd(new SimplePosterization());
                Title = "Simple Posterization";
            } 
            if (numberfilter == 27)
            {
                numberfilter = 0;
                UpdateImage();
            } 
        }

        private void FilterAdd(dynamic filter)
        {
            Cursor = Cursors.Wait;
            try
            {
                using (Bitmap filterbmp = (Bitmap)org.Clone())
                {
                    filter.ApplyInPlace(filterbmp);
                    ImageFilterBox.Source = ImageSourceFromBitmap((Bitmap)filterbmp);
                    filterbmp.Dispose();
                    GC.Collect();
                }                   
            }
            catch
            {
                MessageBox.Show("Format pikseli tego obrazu nie umożliwia wykonania tej operacji!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Cursor = Cursors.Arrow;
        }
        private ImageSource ImageSourceFromBitmap(Bitmap bmp) => Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
}
