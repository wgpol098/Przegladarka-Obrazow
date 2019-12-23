using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Accord.Imaging.Filters;
using Rectangle = System.Drawing.Rectangle;
using Przegladarka_obazow.ValueWindow;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading.Tasks;
using Przegladarka_obazow.Tools.Filters;
using AForge.Imaging;
using Przegladarka_obazow.Tools.Histogram;

namespace Przegladarka_obazow
{
    /// <summary>
    /// Logika interakcji dla klasy Edycja_zdjecia.xaml
    /// </summary>
    public partial class Edycja_zdjecia : Window
    {
        //Do EMGU.CV
        static readonly CascadeClassifier cascadeClasifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        bool FaceDetectionValue = false;
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
            WindowState = WindowState.Maximized;
            imageName = zdj.Source.ToString();
            imageName = imageName.Remove(0, 8);
            imageName = imageName.Replace('/','\\');
            Title = /*"Edycja " + */imageName;

            TransformedBitmap EditedBitMap = new TransformedBitmap();
            EditedBitMap.BeginInit();
            EditedBitMap.Source = (BitmapSource)zdj.Source;
            EditedBitMap.EndInit();

            bitmap = BitmapFromImageSource(zdj.Source);

            ImageEditBox.Source = EditedBitMap;//.CloneCurrentValue();// Clone();
            PreviousImage = bitmap;

            HistogramControlVisible(false);
            FaceDetection();
            Plot1.PlotType = OxyPlot.PlotType.XY;
        }

        //Ze swoich
        private void MenuItem_Click(object sender, RoutedEventArgs e) => FilterAdd(new Negative(0));
        private void MenuItem_Click_14(object sender, RoutedEventArgs e) => FilterAdd(new Negative(1));
        private void MenuItem_Click_15(object sender, RoutedEventArgs e) => FilterAdd(new Negative(2));
        private void MenuItem_Click_16(object sender, RoutedEventArgs e) => FilterAdd(new Negative(3));
        //Wyrownanie histogramu
        private void MenuItem_Click_40(object sender, RoutedEventArgs e) => FilterAdd(new HistogramAlignment());
        //Rozciaganie histogramu
        private void MenuItem_Click_41(object sender, RoutedEventArgs e) => FilterAdd(new HistogramStretching());
        private void MenuItem_Click_3(object sender, RoutedEventArgs e) => FilterAdd(new GrayScale());


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


        //Filtry z AFroge
        private void MenuItem_Click_31(object sender, RoutedEventArgs e) => FilterAdd(new AdaptiveSmoothing());
        private void MenuItem_Click_32(object sender, RoutedEventArgs e) => FilterAdd(new AdditiveNoise());
        private void MenuItem_Click_33(object sender, RoutedEventArgs e) => FilterAdd(new BayerFilter());
        private void MenuItem_Click_34(object sender, RoutedEventArgs e) => FilterAdd(new BilateralSmoothing());
        private void MenuItem_Click_35(object sender, RoutedEventArgs e) => FilterAdd(new Blur());
        private void MenuItem_Click_36(object sender, RoutedEventArgs e) => FilterAdd(new BrightnessCorrection());
        private void MenuItem_Click_37(object sender, RoutedEventArgs e) => FilterAdd(new ConservativeSmoothing());
        private void MenuItem_Click_38(object sender, RoutedEventArgs e) => FilterAdd(new Edges());
        private void MenuItem_Click_39(object sender, RoutedEventArgs e) => FilterAdd(new Pixellate());



        //Modyfikator odcienia
        private void MenuItem_Click_30(object sender, RoutedEventArgs e)
        {
            HueModifierValue dlg = new HueModifierValue();
            dlg.ShowDialog();
            int hueval = dlg.huevalue();
            dlg.Close();
            FilterAdd(new HueModifier(hueval));
        }
        
        private void FaceDetection(bool OpenProperty=false)
        {
            if (FaceDetectionValue == false) return;

            Bitmap tmp = (Bitmap)bitmap.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(tmp);
            Rectangle[] rectangles = cascadeClasifier.DetectMultiScale(grayImage, 1.2, 1);

            int i = 0;
            foreach(Rectangle rectangle in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(tmp))
                {
                    using (System.Drawing.Pen pen= new System.Drawing.Pen(System.Drawing.Color.Red, 1))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                        i++;
                    }
                }
            }
            if(i==0)
                if(OpenProperty == true) MessageBox.Show("Nie znaleziono twarzy na zdjęciu!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information); 
            if(i!=0) ImageEditBox.Source = ImageSourceFromBitmap(tmp);
            tmp.Dispose();
        }
        private void DropImageDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string file in files)
            {
                if (file == imageName) return;
                if (imageModified == true)
                {
                    RoutedEventArgs e2 = new RoutedEventArgs();
                    MenuItem_Click_8(sender, e2);
                }
                Bitmap b = new Bitmap(file);
                bitmap = b;
                PreviousImage = b;
                ImageEditBox.Source = ImageSourceFromBitmap(b);
                
                imageModified = false;
                imageName = file;
                Title = imageName;

                HistogramDraw();
                FaceDetection();              
            } 
        }
        private void DropImageEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
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
            this.Cursor = Cursors.Wait;

            EditedBitMap = new TransformedBitmap();

            EditedBitMap.BeginInit();
            EditedBitMap.Source = (BitmapSource)ImageSourceFromBitmap(bitmap);
            if (value == -1) EditedBitMap.Transform = new RotateTransform(90);
            if (value == 1) EditedBitMap.Transform = new RotateTransform(-90);
            EditedBitMap.EndInit();

            ImageEditBox.Source = EditedBitMap;
            bitmap = BitmapFromImageSource(ImageEditBox.Source);
            if( imageModified == false ) this.Title = this.Title + "*";
            imageModified = true;
            FaceDetection();

            this.Cursor = Cursors.Arrow;
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
            d.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.bmp;*.jpg;*.gif|All files (*.*)|*.*";
            if (d.ShowDialog() == true)
            {
                Bitmap bit = BitmapFromImageSource(ImageEditBox.Source);
                bit.Save(d.FileName);
            }
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
            this.Cursor = Cursors.Wait;
            PreviousImage = (Bitmap)bitmap.Clone();
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            imageModified = true;
            FaceDetection();
            this.Cursor = Cursors.Arrow;
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            Informacje info = new Informacje(ImageEditBox,imageName);
            info.ShowDialog();
        }

        private void Cofnij_Click(object sender, RoutedEventArgs e)
        {
            ImageEditBox.Source = ImageSourceFromBitmap(PreviousImage);
            using (Bitmap tmp = new Bitmap(bitmap))
            {
                bitmap = PreviousImage;
                PreviousImage = (Bitmap)tmp.Clone();
                tmp.Dispose();
            }             
            HistogramDraw();
            FaceDetection();
        }

        //rozjaśnianie
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            PreviousImage = (Bitmap)bitmap.Clone();
            LightValue dlg = new LightValue();
            dlg.ShowDialog();
            int lightvalue = dlg.lightvalue();
            dlg.Close();

            this.Cursor = Cursors.Wait;
            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
                if ((lightvalue + i) > 255) LUT[i] = 255;
                else if ((lightvalue + i) < 0) LUT[i] = 0;
                else LUT[i] = (byte)(lightvalue + i);

            ImageEditBox.Source = SetLUTtoPixel(LUT);
            ImageModified();

            this.Cursor = Cursors.Arrow;
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            if (imageName == "") MenuItem_Click_2(sender, e);
            else
            {
                MessageBoxResult dlg = MessageBox.Show("Zapisać zmiany w pliku?", "Informacja", MessageBoxButton.YesNo);
                if (dlg == MessageBoxResult.Yes)
                {
                    Bitmap bit = BitmapFromImageSource(ImageEditBox.Source);
                    Bitmap b = (Bitmap)bit.Clone();
                    b.Dispose();
                    ImageEditBox.Source = null;
                    String FName = imageName.Replace("\\", @"\");
                    b.Save(FName);
                }
            }
        }

        private void Edycja_zdjecia_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (imageModified == false)
            {
                bitmap.Dispose();
                PreviousImage.Dispose();
                ImageEditBox.Source = null;
                return;
            }
            RoutedEventArgs e2 = new RoutedEventArgs();
            MenuItem_Click_8(sender, e2);
            bitmap.Dispose();
            PreviousImage.Dispose();
            ImageEditBox.Source = null;
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
                PreviousImage = BitmapFromImageSource(ImageEditBox.Source);
                bitmap = PreviousImage;
                imageName = ImageEditBox.Source.ToString();
                imageName = imageName.Remove(0, 8);
                imageName = imageName.Replace('/', '\\');
                this.Title = imageName;
            }
            HistogramDraw();
            FaceDetection();
        }

        //Korekcja gamma
        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            PreviousImage = (Bitmap)bitmap.Clone();
            double gammavalue = 1.1;

            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
                if ((255 * Math.Pow(i / 255.0, 1 / gammavalue)) > 255) LUT[i] = 255;
                else LUT[i] = (byte)(255 * Math.Pow(i / 255.0, 1 / gammavalue));

            ImageEditBox.Source = SetLUTtoPixel(LUT);
            ImageModified();
            this.Cursor = Cursors.Arrow;
        }

        //Korekcja kontrastu
        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
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
            ImageModified();

            this.Cursor = Cursors.Arrow;
        }

        private ImageSource SetLUTtoPixel(byte[] LUT)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            Parallel.For(0, pixelValues.Length, i =>
            {
                pixelValues[i] = LUT[pixelValues[i]];
            }); 

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
            return ImageSourceFromBitmap(bitmap);
        }

        private void HistogramDraw()
        {
            
            if (histogramView == false) return;

            this.Cursor = Cursors.Wait;

            Array.Clear(HistogramAll, 0, 256);
            Array.Clear(HistogramR, 0, 256);
            Array.Clear(HistogramG, 0, 256);
            Array.Clear(HistogramB, 0, 256);

            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            int width = bitmap.Width;
            int height = bitmap.Height;
            Parallel.For(0, height, y =>
             {
                 Parallel.For(0, width, x =>
                  {
                      int index = y * bmpData.Stride + x * bytesPerPixel;
                      HistogramR[pixelValues[index + 2]]++;
                      HistogramG[pixelValues[index + 1]]++;
                      HistogramB[pixelValues[index]]++;
                  });
             });
                
            for(int i=0;i<256;i++)
                HistogramAll[i] = HistogramR[i] + HistogramG[i] + HistogramB[i];
            
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);

            FillHistogram();
            this.Cursor = Cursors.Arrow;
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
            ImageModified();
        }
       
        private void FilterAdd(dynamic filter)
        {
            this.Cursor = Cursors.Wait;
            try
            {
                PreviousImage = (Bitmap)bitmap.Clone();
                filter.ApplyInPlace(bitmap);
                ImageEditBox.Source = ImageSourceFromBitmap(bitmap);              
                ImageModified();
            }
            catch
            {
                MessageBox.Show("Format pikseli tego obrazu nie umożliwia wykonania tej operacji!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Cursor = Cursors.Arrow;
        }

        private void MenuItemFaceDetection_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;

            if (FaceDetectionValue == true)
            {
                FaceDetectionValue = false;
                ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            } 
            else FaceDetectionValue = true;
            FaceDetection(true);

            this.Cursor = Cursors.Arrow;
        }

        private void ImageModified()
        {
            if (imageModified == false) this.Title = this.Title + "*";
            imageModified = true;
            FaceDetection();
            HistogramDraw();
        }

        //Obsługa Klawiszy
        private void myControl_KeyDown(object sender, KeyEventArgs e)
        {
            RoutedEventArgs tmp = new RoutedEventArgs();

            //Cofanie zmian
            if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
                Cofnij_Click(sender,tmp);
            //Otwieranie pliku
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
                MenuItem_Click_9(sender,tmp);

            //Zapisz
            if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)        
                MenuItem_Click_8(sender, tmp);

            //Zapisz jako...
            if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.S)
                MenuItem_Click_2(sender, tmp);

            //Wyjdz
            if (e.Key == Key.Escape)
                MenuItem_Click_1(sender, tmp);

            //Histogram
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.H)
            {
                MenuItem_Click_12(sender, tmp);
                if (MenuItemHistagram.IsChecked == false) MenuItemHistagram.IsChecked = true;
                else MenuItemHistagram.IsChecked = false;
            }
                
            //Wykrywanie twarzy
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F)
            {
                MenuItemFaceDetection_Click(sender, tmp);
                if (MenuItemFaceDetection.IsChecked == false) MenuItemFaceDetection.IsChecked = true;
                else MenuItemFaceDetection.IsChecked = false;

            }

            //Informacje
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.I)
                MenuItem_Click_5(sender, tmp);

        }
    }
}