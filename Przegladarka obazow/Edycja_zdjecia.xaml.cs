using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using Accord.Imaging.Filters;
using Rectangle = System.Drawing.Rectangle;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.EdgeDetection;

namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy Edycja_zdjecia.xaml
    /// </summary>
    public partial class Edycja_zdjecia : Window
    {
        //Zmienna przechowująca nazwę pliku w pamięci
        string imageName = "";
        //Zmienne potrzebne do przechowywania w pamięci zmian w obrazie
        TransformedBitmap EditedBitMap;
        Bitmap PreviousImage;
        Bitmap bitmap;
        //Zmienna przechowyjąca informację czy obraz był modyfikowany
        bool imageModified = false;
        //Zmienna przechowująca informację czy histagram jest widoczny czy nie
        bool histogramView = false;
        //Zmienna przechowująca jaki kolor maluje aktualnie histogram
        int histogramColorValue = 0;
        //Tablice do przechowywania informacji o histogramie
        int[] HistogramAll = new int[256];
        int[] HistogramR = new int[256];
        int[] HistogramG = new int[256];
        int[] HistogramB = new int[256];
        public Edycja_zdjecia(System.Windows.Controls.Image zdj)
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;
            imageName = zdj.Source.ToString();
            imageName = imageName.Remove(0, 8);
            Title = /*"Edycja " + */imageName;

            TransformedBitmap EditedBitMap = new TransformedBitmap();
            EditedBitMap.BeginInit();
            EditedBitMap.Source = (BitmapSource)zdj.Source;
            EditedBitMap.EndInit();
            bitmap = BitmapFromImageSource(zdj.Source.Clone());

            ImageEditBox.Source = EditedBitMap;//.CloneCurrentValue();// Clone();
            PreviousImage = bitmap;

            //Histogram
            HistogramControlVisible(false);
            Plot1.PlotType = OxyPlot.PlotType.XY;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) => Negative(0);
        private void MenuItem_Click_14(object sender, RoutedEventArgs e) => Negative(1);
        private void MenuItem_Click_15(object sender, RoutedEventArgs e) => Negative(2);
        private void MenuItem_Click_16(object sender, RoutedEventArgs e) => Negative(3);


        private void MenuItem_Click_1(object sender, RoutedEventArgs e) => Close();


        //Filtry z Accord.Imagining
        private void MenuItem_Click_6(object sender, RoutedEventArgs e) => FilterAdd(new Sepia());
        //Filtr Obraz olejny
        private void MenuItem_Click_17(object sender, RoutedEventArgs e) => FilterAdd(new OilPainting());
        //Pixeloza
        private void MenuItem_Click_18(object sender, RoutedEventArgs e) => FilterAdd(new Pixellate());
        //Filtr środkowoprzepustowy
        private void MenuItem_Click_19(object sender, RoutedEventArgs e) => FilterAdd(new Median());
        //Mean filter
        private void MenuItem_Click_20(object sender, RoutedEventArgs e) => FilterAdd(new Mean());
        //Jitter filter
        private void MenuItem_Click_21(object sender, RoutedEventArgs e) => FilterAdd(new Jitter());
        //Exponential filter
        private void MenuItem_Click_22(object sender, RoutedEventArgs e) => FilterAdd(new Exponential());
        //Filtrowanie euklidesowe
        private void MenuItem_Click_23(object sender, RoutedEventArgs e) => FilterAdd(new EuclideanColorFiltering());
        //AdditiveNoise
        private void MenuItem_Click_24(object sender, RoutedEventArgs e) => FilterAdd(new AdditiveNoise());
        //TopHat
        private void MenuItem_Click_25(object sender, RoutedEventArgs e) => FilterAdd(new TopHat());
        //Filtr wyostrzający
        private void MenuItem_Click_26(object sender, RoutedEventArgs e) => FilterAdd(new Sharpen());
        //SaltAndPepperNoise
        private void MenuItem_Click_27(object sender, RoutedEventArgs e) => FilterAdd(new SaltAndPepperNoise());
        //Chromatyczność RG
        private void MenuItem_Click_28(object sender, RoutedEventArgs e) => FilterAdd(new RGChromacity());
        //Filtr logarytmiczny
        private void MenuItem_Click_29(object sender, RoutedEventArgs e) => FilterAdd(new Logarithm());
        //Modyfikator odcienia
        private void MenuItem_Click_30(object sender, RoutedEventArgs e) => FilterAdd(new HueModifier(350));

        private void DropImage(object sender, DragEventArgs e)
        {
            MessageBox.Show("Powinno dzialac ale nie dziala!");
            //MessageBox.Show(e.Data.GetDataPresent(typeof()).ToString());
            //if(e.Data.GetDataPresent(typeof(Bitmap)))
            {
                //System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
                //img.Source = ImageSourceFromBitmap((Bitmap)e.Data.GetData(typeof(Bitmap)));
                //img.Source = (BitmapSource)e.Data.GetData(typeof(System.Windows.Controls.Image));

                //ImageEditBox.Source = img.Source;
            }
        }
        private void ObrotWPrawo_Click(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            Obrot(1);
        }
        private void ObrotWLewo_Click(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            Obrot(-1);
        }
        private void Obrot(int value)
        {
            EditedBitMap = new TransformedBitmap();

            EditedBitMap.BeginInit();
            EditedBitMap.Source = (BitmapSource)ImageEditBox.Source;
            if (value == -1) EditedBitMap.Transform = new RotateTransform(90);
            if (value == 1) EditedBitMap.Transform = new RotateTransform(-90);
            EditedBitMap.EndInit();

            ImageEditBox.Source = EditedBitMap;
            bitmap = BitmapFromImageSource(ImageEditBox.Source);
            imageModified = true;

        }

        private void Negative(int val)
        {
            PreviousImage = (Bitmap)bitmap.Clone();

            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int index = y * bmpData.Stride + x * bytesPerPixel;
                    if (val == 0 || val == 1) pixelValues[index + 2] = (byte)(255 - pixelValues[index + 2]);
                    if (val == 0 || val == 2) pixelValues[index + 1] = (byte)(255 - pixelValues[index + 1]);
                    if (val == 0 || val == 3) pixelValues[index] = (byte)(255 - pixelValues[index]);
                }
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);

            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);

            imageModified = true;
            HistogramDraw();
        }

        private ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private Bitmap BitmapFromImageSource(ImageSource img)
        {
            Stream stream = new MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)img));
            pngEncoder.Save(stream);
            return new Bitmap(stream);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            if (d.ShowDialog() == true)
            {
                Bitmap bit = BitmapFromImageSource(ImageEditBox.Source);
                bit.Save(d.FileName);
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();

            FormatConvertedBitmap format = new FormatConvertedBitmap();

            format.BeginInit();
            format.Source = (BitmapSource)ImageEditBox.Source;
            format.DestinationFormat = PixelFormats.Gray32Float;
            format.EndInit();

            ImageEditBox.Source = format;
            bitmap = BitmapFromImageSource(ImageEditBox.Source);

            imageModified = true;
            HistogramDraw();
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            //Sprawdz czy prawym czy lewym
            if (e.ClickCount == 2) MessageBox.Show("Działa");
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            Obrot(1);
            Obrot(1);
        }

        private void ObrotWPoziomie_Click(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            imageModified = true;
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            Informacje info = new Informacje(ImageEditBox);
            info.ShowDialog();
        }

        private void Cofnij_Click(object sender, RoutedEventArgs e)
        {
            ImageEditBox.Source = ImageSourceFromBitmap(PreviousImage);
            bitmap = PreviousImage;
            HistogramDraw();
        }

        //rozjaśnianie
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            LightValue dlg = new LightValue();
            dlg.ShowDialog();
            int lightvalue = dlg.lightvalue();
            dlg.Close();

            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
                if ((lightvalue + i) > 255) LUT[i] = 255;
                else if ((lightvalue + i) < 0) LUT[i] = 0;
                else LUT[i] = (byte)(lightvalue + i);

            ImageEditBox.Source = SetLUTtoPixel(LUT);
            imageModified = true;
            HistogramDraw();
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            MenuItem_Click_2(sender, e);

            if (imageName == "") MenuItem_Click_2(sender, e);
            else
            {
                MessageBoxResult dlg = MessageBox.Show("Zapisać zmiany w pliku?", "Informacja", MessageBoxButton.YesNo);
                if (dlg == MessageBoxResult.Yes)
                {
                    Bitmap bit = BitmapFromImageSource(ImageEditBox.Source);
                    Bitmap b = new Bitmap(bit);
                    bit.Dispose();
                    ImageEditBox.Source = null;
                    bit.Save(imageName);
                }
            }
        }

        private void Edycja_zdjecia_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (imageModified == false) return;

            RoutedEventArgs e2 = new RoutedEventArgs();
            System.Windows.Forms.DialogResult dlg = System.Windows.Forms.MessageBox.Show("Zatwierdzić niezapisane zmiany?", "Ostrzeżenie!", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            if (dlg == System.Windows.Forms.DialogResult.Yes) MenuItem_Click_8(sender, e2);
        }

        //Otwieranie pliku
        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            if (d.ShowDialog() == true)
            {
                System.Windows.Controls.Image bit = new System.Windows.Controls.Image();
                bit.Source = new BitmapImage(new Uri(d.FileName));
                ImageEditBox.Source = bit.Source;
            }
            HistogramDraw();
        }

        //Korekcja gamma
        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            double gammavalue = 1.1;

            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
                if ((255 * Math.Pow(i / 255.0, 1 / gammavalue)) > 255) LUT[i] = 255;
                else LUT[i] = (byte)(255 * Math.Pow(i / 255.0, 1 / gammavalue));

            ImageEditBox.Source = SetLUTtoPixel(LUT);
            imageModified = true;
            HistogramDraw();
        }

        //Korekcja kontrastu
        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            double korekcjavalue = 30;
            byte[] LUT = new byte[256];
            double a;

            if (korekcjavalue <= 0) a = 1.0 + (korekcjavalue / 256.0);
            else a = 256.0 / Math.Pow(2, Math.Log(257 - korekcjavalue, 2));
            for (int i = 0; i < 256; i++)
                if ((a * (i - 127) + 127) > 255) LUT[i] = 255;
                else if ((a * (i - 127) + 127) < 0) LUT[i] = 0;
                else LUT[i] = (byte)(a * (i - 127) + 127);

            ImageEditBox.Source = SetLUTtoPixel(LUT);
            imageModified = true;
            HistogramDraw();
        }

        private ImageSource SetLUTtoPixel(byte[] LUT)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            for (int i = 0; i < pixelValues.Length; i++)
                pixelValues[i] = LUT[pixelValues[i]];

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
            return ImageSourceFromBitmap(bitmap);
        }

        private void HistogramDraw()
        {
            if (histogramView == false) return;

            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            for (int i = 0; i < 256; i++)
            {
                HistogramAll[i] = 0;
                HistogramR[i] = 0;
                HistogramG[i] = 0;
                HistogramB[i] = 0;
            }

            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int index = y * bmpData.Stride + x * bytesPerPixel;
                    HistogramR[pixelValues[index + 2]]++;
                    HistogramG[pixelValues[index + 1]]++;
                    HistogramB[pixelValues[index]]++;

                    HistogramAll[pixelValues[index + 2]]++;
                    HistogramAll[pixelValues[index + 1]]++;
                    HistogramAll[pixelValues[index]]++;

                }
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);

            FillHistogram();
        }

        private void FillHistogram()
        {
            IList<OxyPlot.DataPoint> Points;
            Points = new List<OxyPlot.DataPoint>();

            for (int i = 1; i < 255; i++)
            {
                if (histogramColorValue == 0) Points.Add(new OxyPlot.DataPoint(i, HistogramAll[i]));
                if (histogramColorValue == 1) Points.Add(new OxyPlot.DataPoint(i, HistogramR[i]));
                if (histogramColorValue == 2) Points.Add(new OxyPlot.DataPoint(i, HistogramG[i]));
                if (histogramColorValue == 3) Points.Add(new OxyPlot.DataPoint(i, HistogramB[i]));
            }
            no.ItemsSource = Points;
        }


        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {
            if (histogramView == false) HistogramControlVisible(true);
            else HistogramControlVisible(false);
        }

        private void HistogramControlVisible(bool val)
        {
            if (val == false)
            {
                ImageEditBox.Margin = new Thickness(ImageEditBox.Margin.Left, ImageEditBox.Margin.Top, ImageEditBox.Margin.Right - 315, ImageEditBox.Margin.Bottom);
                Plot1.Visibility = Visibility.Hidden;
                ButtonAll.Visibility = Visibility.Hidden;
                ButtonR.Visibility = Visibility.Hidden;
                ButtonG.Visibility = Visibility.Hidden;
                ButtonB.Visibility = Visibility.Hidden;
                histogramView = val;
            }
            if (val == true)
            {
                ImageEditBox.Margin = new Thickness(ImageEditBox.Margin.Left, ImageEditBox.Margin.Top, ImageEditBox.Margin.Right + 315, ImageEditBox.Margin.Bottom);
                Plot1.Visibility = Visibility.Visible;
                ButtonAll.Visibility = Visibility.Visible;
                ButtonR.Visibility = Visibility.Visible;
                ButtonG.Visibility = Visibility.Visible;
                ButtonB.Visibility = Visibility.Visible;
                histogramView = val;
                HistogramDraw();
            }
        }

        private void ButtonAll_Click(object sender, RoutedEventArgs e)
        {
            histogramColorValue = 0;
            FillHistogram();
        }

        private void ButtonR_Click(object sender, RoutedEventArgs e)
        {
            histogramColorValue = 1;
            FillHistogram();
        }

        private void ButtonG_Click(object sender, RoutedEventArgs e)
        {
            histogramColorValue = 2;
            FillHistogram();
        }

        private void ButtonB_Click(object sender, RoutedEventArgs e)
        {
            histogramColorValue = 3;
            FillHistogram();
        }

        //Zmiana rozdzielczości 
        private void MenuItem_Click_13(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap;
            ImageResizeValue dlg = new ImageResizeValue(ImageEditBox);
            dlg.ShowDialog();

            BitmapImage bmp = new BitmapImage();

            Stream stream = new MemoryStream();
            var pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create((BitmapSource)ImageEditBox.Source));
            pngEncoder.Save(stream);

            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.DecodePixelWidth = dlg.GetWidth();
            bmp.DecodePixelHeight = dlg.GetHeight();
            bmp.EndInit();

            dlg.Close();

            ImageEditBox.Source = bmp;
            bitmap = BitmapFromImageSource(ImageEditBox.Source);
            imageModified = true;
            HistogramDraw();
        }
       
        private void FilterAdd(dynamic filter)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            filter.ApplyInPlace(bitmap);
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            imageModified = true;
            HistogramDraw();
        }
    }
}