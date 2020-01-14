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
using Przegladarka_obazow.Tools.Modifications;
using Przegladarka_obazow.Tools.LUT;
using Tesseract;
using Przegladarka_obazow.Tools.Options;
using Przegladarka_obazow.AppWindow;

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
        Bitmap PreviousImage;
        Bitmap bitmap;
        //Zmienna przechowyjąca informację czy obraz był modyfikowany
        bool imageModified = false;
        //Zmienna przechowująca informację czy histagram jest widoczny czy nie
        bool histogramView = false;
        //Zmienna przechowująca informację czy oryginalne zdjęcie jest widoczne czy nie
        bool originalPictureView = false;
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

            bitmap = BitmapFromImageSource(zdj.Source);
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            PreviousImage = (Bitmap)bitmap.Clone();
            ImageOriginalBox.Source = ImageEditBox.Source;

            ImageOriginalVisible(false,true);
            HistogramControlVisible(false);
            FaceDetection();
            Plot1.PlotType = OxyPlot.PlotType.XY;
        }

        //Ze swoich
        private void NegativeAll_Click(object sender, RoutedEventArgs e) => FilterAdd(new Negative(0));
        private void NegativeR_Click(object sender, RoutedEventArgs e) => FilterAdd(new Negative(1));
        private void NegativeG_Click(object sender, RoutedEventArgs e) => FilterAdd(new Negative(2));
        private void NegativeB_Click(object sender, RoutedEventArgs e) => FilterAdd(new Negative(3));
        private void MenuItem_Click_40(object sender, RoutedEventArgs e) => FilterAdd(new HistogramAlignment());
        private void MenuItem_Click_41(object sender, RoutedEventArgs e) => FilterAdd(new HistogramStretching());
        private void MenuItem_Click_3(object sender, RoutedEventArgs e) => FilterAdd(new GrayScale());
        private void MenuItem_Click_42(object sender, RoutedEventArgs e) => FilterAdd(new GrayScaleToRGB());
        private void MenuItem_Click_44(object sender, RoutedEventArgs e) => FilterAdd(new Tools.Filters.RotateChannels());


        private void MenuItem_Click_1(object sender, RoutedEventArgs e) => Close();


        //Filtry z Accord.Imagining
        private void MenuItem_Click_6(object sender, RoutedEventArgs e) => FilterAdd(new Sepia());
        private void MenuItem_Click_17(object sender, RoutedEventArgs e) => FilterAdd(new OilPainting());
        private void MenuItem_Click_18(object sender, RoutedEventArgs e) => FilterAdd(new Pixellate());
        private void MenuItem_Click_19(object sender, RoutedEventArgs e) => FilterAdd(new Median());
        private void MenuItem_Click_20(object sender, RoutedEventArgs e) => FilterAdd(new Mean());
        private void MenuItem_Click_21(object sender, RoutedEventArgs e) => FilterAdd(new Jitter());
        private void MenuItem_Click_22(object sender, RoutedEventArgs e) => FilterAdd(new Exponential());
        private void MenuItem_Click_23(object sender, RoutedEventArgs e) => FilterAdd(new EuclideanColorFiltering());
        private void MenuItem_Click_24(object sender, RoutedEventArgs e) => FilterAdd(new AdditiveNoise());
        private void MenuItem_Click_25(object sender, RoutedEventArgs e) => FilterAdd(new TopHat());
        private void MenuItem_Click_26(object sender, RoutedEventArgs e) => FilterAdd(new Sharpen());
        private void MenuItem_Click_27(object sender, RoutedEventArgs e) => FilterAdd(new SaltAndPepperNoise());
        private void MenuItem_Click_28(object sender, RoutedEventArgs e) => FilterAdd(new RGChromacity());
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
        private void MenuItem_Click_45(object sender, RoutedEventArgs e) => FilterAdd(new Sharpen());
        private void MenuItem_Click_46(object sender, RoutedEventArgs e) => FilterAdd(new SimplePosterization());

        //Zamiana danego koloru
        private void MenuItem_Click_52(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dlg1 = new System.Windows.Forms.ColorDialog();
            System.Windows.Forms.ColorDialog dlg2 = new System.Windows.Forms.ColorDialog();

            if (dlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                if (dlg2.ShowDialog() == System.Windows.Forms.DialogResult.OK) FilterAdd(new ReplaceColor(dlg1.Color.R, dlg1.Color.R, dlg2.Color.B, dlg2.Color.R, dlg2.Color.G, dlg2.Color.B));
        }
        //Izolacja danego koloru
        private void MenuItem_Click_50(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            if(dlg.ShowDialog()==System.Windows.Forms.DialogResult.OK) FilterAdd(new IsolateColor(dlg.Color.R, dlg.Color.G, dlg.Color.B));
        }
        //Usuwanie danego koloru
        private void MenuItem_Click_49(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            if(dlg.ShowDialog()==System.Windows.Forms.DialogResult.OK) FilterAdd(new DeleteColor(dlg.Color.R, dlg.Color.G, dlg.Color.B));
        }

        //Modyfikator odcienia
        private void MenuItem_Click_30(object sender, RoutedEventArgs e)
        {
            HueModifierValue dlg = new HueModifierValue();
            dlg.ShowDialog();
            if(dlg.ModifiedStatus()==true) FilterAdd(new HueModifier(dlg.huevalue()));
            dlg.Close();
        }

        //Binaryzacja obrazu
        private void MenuItem_Click_43(object sender, RoutedEventArgs e)
        {
            BinarizationValue dlg = new BinarizationValue();
            dlg.ShowDialog();
            if(dlg.ModifiedStatus()==true) FilterAdd(new Binarization(dlg.binarizationvalue(),dlg.tresholdvalue()));
            dlg.Close();
        }

        //Wartość pikseli
        private void MenuItem_Click_47(object sender, RoutedEventArgs e)
        {
            PixelsRangeValue dlg = new PixelsRangeValue();
            dlg.ShowDialog();
            if(dlg.ModifiedStatus()==true) FilterAdd(new PixelsRange(dlg.minrangevalue(), dlg.maxrangevalue(), dlg.rvalue(), dlg.gvalue(), dlg.bvalue()));
            dlg.Close();
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
                        pen.Dispose();
                    }
                    graphics.Dispose();
                }   
            }
            if(i==0)
                if(OpenProperty == true) MessageBox.Show("Nie znaleziono twarzy na zdjęciu!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information); 
            if(i!=0) ImageEditBox.Source = ImageSourceFromBitmap(tmp);
            grayImage.Dispose();
            tmp.Dispose();
            GC.Collect(0, GCCollectionMode.Forced);
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
                ImageOriginalBox.Source = ImageSourceFromBitmap(b);
                
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
        private void ObrotWPrawo_Click(object sender, RoutedEventArgs e) => Rotate(1);
        private void ObrotWLewo_Click(object sender, RoutedEventArgs e) => Rotate(-1);
        private void ObrotWPionie_Click(object sender, RoutedEventArgs e) => Rotate(0);
        private void ObrotWPoziomie_Click(object sender, RoutedEventArgs e) => Rotate(2);

        private void Rotate(int value)
        {
            Cursor = Cursors.Wait;

            IntPtr hBitmap = PreviousImage.GetHbitmap();
            PreviousImage = (Bitmap)bitmap.Clone();
            if(value == 1) bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY);
            if(value == -1) bitmap.RotateFlip(RotateFlipType.Rotate270FlipXY);
            if (value == 0) bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            if (value == 2) bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
                        
            DeleteObject(hBitmap);
            if (imageModified == false) this.Title = this.Title + "*";
            imageModified = true;
            FaceDetection();

            GC.Collect(0, GCCollectionMode.Forced);
            Cursor = Cursors.Arrow;
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
            if (e.LeftButton == MouseButtonState.Pressed)
                if (e.ClickCount == 2) FastFilterAdd();
        }

        private void Informations_Click(object sender, RoutedEventArgs e)
        {
            Informacje info = new Informacje(ImageEditBox,imageName);
            info.ShowDialog();
            info.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ImageEditBox.Source = ImageSourceFromBitmap(PreviousImage);
            using (Bitmap tmp = bitmap)
            {
                bitmap = PreviousImage;
                PreviousImage = (Bitmap)tmp.Clone();
                tmp.Dispose();
            }           
            HistogramDraw();
            FaceDetection();
            GC.Collect(0, GCCollectionMode.Forced);
        }

        //rozjaśnianie
        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {         
            LightValue dlg = new LightValue();
            dlg.ShowDialog();

            Cursor = Cursors.Wait;
            if (dlg.ModifiedStatus()==true)
            {
                IntPtr hBitmap = PreviousImage.GetHbitmap();
                PreviousImage = (Bitmap)bitmap.Clone();
                byte[] LUT = LutModifications.LightLUT(dlg.lightvalue());
                LutModifications.SetLUTToBitmap(bitmap, LUT);
                ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
                DeleteObject(hBitmap);
                ImageModified();
                GC.Collect(0, GCCollectionMode.Forced);
            }
            dlg.Close();
            Cursor = Cursors.Arrow;
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
                GC.Collect(0, GCCollectionMode.Forced);
                return;
            }
            RoutedEventArgs e2 = new RoutedEventArgs();
            MenuItem_Click_8(sender, e2);
            bitmap.Dispose();
            PreviousImage.Dispose();
            ImageEditBox.Source = null;
            GC.Collect(0, GCCollectionMode.Forced);
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
                IntPtr hBitmap1 = PreviousImage.GetHbitmap();
                IntPtr hBitmap2 = bitmap.GetHbitmap();
                bitmap = PreviousImage;
                imageName = ImageEditBox.Source.ToString();
                imageName = imageName.Remove(0, 8);
                imageName = imageName.Replace('/', '\\');
                Title = imageName;
                DeleteObject(hBitmap1);
                DeleteObject(hBitmap2);
                GC.Collect(0, GCCollectionMode.Forced);
            }
            HistogramDraw();
            FaceDetection();
        }

        //Korekcja gamma
        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            IntPtr hBitmap = PreviousImage.GetHbitmap();
            PreviousImage = (Bitmap)bitmap.Clone();
            double gammavalue = 1.1;
            byte[] LUT = LutModifications.GammaLUT(gammavalue);
            LutModifications.SetLUTToBitmap(bitmap, LUT);
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            ImageModified();
            DeleteObject(hBitmap);
            GC.Collect(0, GCCollectionMode.Forced);
            Cursor = Cursors.Arrow;
        }

        //Korekcja kontrastu
        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            IntPtr hBitmap = PreviousImage.GetHbitmap();
            PreviousImage = (Bitmap)bitmap.Clone();
            double correctionvalue = 30;
            byte[] LUT = LutModifications.CorrectionLUT(correctionvalue);
            LutModifications.SetLUTToBitmap(bitmap, LUT);
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            ImageModified();
            DeleteObject(hBitmap);
            GC.Collect(0, GCCollectionMode.Forced);
            Cursor = Cursors.Arrow;
        }

        private void HistogramDraw()
        {           
            if (histogramView == false) return;

            Cursor = Cursors.Wait;

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
                          
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);

            for (int i = 0; i < 256; i++)
                HistogramAll[i] = HistogramR[i] + HistogramG[i] + HistogramB[i];

            FillHistogram();
            Cursor = Cursors.Arrow;
        }

        private void FillHistogram()
        {
            no.ItemsSource = null;
            IList<OxyPlot.DataPoint> Points;
            Points = new List<OxyPlot.DataPoint>();

            for (byte i = 1; i < 255; i++)
            {
                if (histogramColorValue == 0) Points.Add(new OxyPlot.DataPoint(i, HistogramAll[i]));
                if (histogramColorValue == 1) Points.Add(new OxyPlot.DataPoint(i, HistogramR[i]));
                if (histogramColorValue == 2) Points.Add(new OxyPlot.DataPoint(i, HistogramG[i]));
                if (histogramColorValue == 3) Points.Add(new OxyPlot.DataPoint(i, HistogramB[i]));
            }
            no.ItemsSource = Points;
        }

        private void MouseEnterImageOriginalBox(object sender, MouseEventArgs e)
        {
            ImageEditBox.Source = ImageOriginalBox.Source;
        }
        private void MouseLeaveImageOriginalBox(object sender, MouseEventArgs e)
        {
            ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
        }
        private void MenuItemHistogram_Click(object sender, RoutedEventArgs e)
        {
            if (histogramView == false) HistogramControlVisible(true);
            else HistogramControlVisible(false);
        }
        private void MenuItemOriginalImage_Click(object sender, RoutedEventArgs e)
        {
            if (originalPictureView == false) ImageOriginalVisible(true);
            else ImageOriginalVisible(false);
        }

        private void ImageOriginalVisible(bool val, bool initt = false)
        {
            if(val == false)
            {
                if(histogramView == false && initt == false) ImageEditBox.Margin = new Thickness(ImageEditBox.Margin.Left, ImageEditBox.Margin.Top, ImageEditBox.Margin.Right - 315, ImageEditBox.Margin.Bottom);               
                ImageOriginalBox.Visibility = Visibility.Hidden;
                originalPictureView = val;
            }
            if(val == true)
            {
                if(histogramView == false && initt == false) ImageEditBox.Margin = new Thickness(ImageEditBox.Margin.Left, ImageEditBox.Margin.Top, ImageEditBox.Margin.Right + 315, ImageEditBox.Margin.Bottom);
                if (histogramView == false) ImageOriginalBox.Margin = new Thickness(ImageOriginalBox.Margin.Left, 32, ImageOriginalBox.Margin.Right, ImageOriginalBox.Margin.Bottom);
                if(histogramView == true) ImageOriginalBox.Margin = new Thickness(ImageOriginalBox.Margin.Left, 229, ImageOriginalBox.Margin.Right, ImageOriginalBox.Margin.Bottom);
                ImageOriginalBox.Visibility = Visibility.Visible;
                originalPictureView = val;
                
            }
        }
        private void HistogramControlVisible(bool val, bool initt = false)
        {
            if (val == false)
            {
                if(originalPictureView == false && initt == false) ImageEditBox.Margin = new Thickness(ImageEditBox.Margin.Left, ImageEditBox.Margin.Top, ImageEditBox.Margin.Right - 315, ImageEditBox.Margin.Bottom);
                if(originalPictureView == true) ImageOriginalBox.Margin = new Thickness(ImageOriginalBox.Margin.Left, 32, ImageOriginalBox.Margin.Right, ImageOriginalBox.Margin.Bottom);
                Plot1.Visibility = Visibility.Hidden;
                ButtonAll.Visibility = Visibility.Hidden;
                ButtonR.Visibility = Visibility.Hidden;
                ButtonG.Visibility = Visibility.Hidden;
                ButtonB.Visibility = Visibility.Hidden;
                histogramView = val;
            }
            if (val == true)
            {
                if(originalPictureView == false && initt == false) ImageEditBox.Margin = new Thickness(ImageEditBox.Margin.Left, ImageEditBox.Margin.Top, ImageEditBox.Margin.Right + 315, ImageEditBox.Margin.Bottom);
                if(originalPictureView == true) ImageOriginalBox.Margin = new Thickness(ImageOriginalBox.Margin.Left, 229, ImageOriginalBox.Margin.Right, ImageOriginalBox.Margin.Bottom);
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
            ImageResizeValue dlg = new ImageResizeValue(ImageEditBox);
            dlg.ShowDialog();

            if(dlg.ModifiedStatus()==true)
            {
                IntPtr hBitmap = PreviousImage.GetHbitmap();
                DeleteObject(hBitmap);
                PreviousImage = bitmap;
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

                ImageEditBox.Source = bmp;
                bitmap = BitmapFromImageSource(ImageEditBox.Source);
                ImageModified();
                GC.Collect(0, GCCollectionMode.Forced);
            }
            dlg.Close();
        }

        private void FilterAdd(dynamic filter)
        {
            Cursor = Cursors.Wait;
            try
            {
                IntPtr hBitmap = PreviousImage.GetHbitmap();
                PreviousImage = (Bitmap)bitmap.Clone();
                filter.ApplyInPlace(bitmap);
                ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
                ImageModified();
                DeleteObject(hBitmap);
                GC.Collect(0, GCCollectionMode.Forced);
            }
            catch
            {
                MessageBox.Show("Format pikseli tego obrazu nie umożliwia wykonania tej operacji!", "Błąd!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Cursor = Cursors.Arrow;
        }

        private void MenuItemFaceDetection_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            if (FaceDetectionValue == true)
            {
                FaceDetectionValue = false;
                ImageEditBox.Source = ImageSourceFromBitmap(bitmap);
            } 
            else FaceDetectionValue = true;
            FaceDetection(true);

            Cursor = Cursors.Arrow;
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
                Back_Click(sender,tmp);

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
                MenuItemHistogram_Click(sender, tmp);
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
                Informations_Click(sender, tmp);
        }

        private void MenuItem_Click_48(object sender, RoutedEventArgs e)
        {
            var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractAndCube);
            var page = ocr.Process(bitmap);
            MessageBox.Show(page.GetText());
            ocr.Dispose();
        }

        private void MenuItem_Click_51(object sender, RoutedEventArgs e) => FastFilterAdd();

        private void FastFilterAdd()
        {
            FastFilter dlg = new FastFilter(bitmap);
            dlg.ShowDialog();
            if (dlg.ModifiedStatus() == true)
            {
                IntPtr hBitmap = PreviousImage.GetHbitmap();
                PreviousImage = (Bitmap)bitmap.Clone();
                ImageEditBox.Source = dlg.NewFilterSource();
                bitmap = BitmapFromImageSource(ImageEditBox.Source);
                ImageModified();
                DeleteObject(hBitmap);
            }
            dlg.Close();
            GC.Collect(0, GCCollectionMode.Forced);
        }

        private void GetImagePixelColor(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.Point point = Mouse.GetPosition(ImageEditBox);            
            //System.Drawing.Color cl = bitmap.GetPixel((int)((point.X / ImageEditBox.ActualWidth) * bitmap.Width), (int)((point.Y / ImageEditBox.ActualHeight) * bitmap.Height));
            //MessageBox.Show(cl.R + " " + cl.G + " " + cl.B);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}