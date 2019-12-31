using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Filters
{
    public class Binarization
    {
        //value is 0-255
        int value = 0;
        //1 or -1
        int treshold = -1;

        public Binarization(int _value, int _theshold)
        {
            value = _value;
            treshold = _theshold;
        }

        public void ApplyInPlace(Bitmap bitmap)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            int size = bitmap.Width * bitmap.Height;
            Parallel.For(0, size, i =>
            {
                byte valueGS = (byte)(0.299 * pixelValues[3 * i + 2] + 0.587 * pixelValues[3 * i + 1] + 0.114 * pixelValues[3 * i]);
                if(value > valueGS)
                {
                    //Górny próg
                    if(treshold == 1)
                    {
                        pixelValues[3 * i] = 0;
                        pixelValues[3 * i + 1] = 0;
                        pixelValues[3 * i + 2] = 0;
                    }

                    //Dolny próg
                    if(treshold == -1)
                    {
                        pixelValues[3 * i] = 255;
                        pixelValues[3 * i + 1] = 255;
                        pixelValues[3 * i + 2] = 255;
                    }
                    
                }
                else
                {
                    if(treshold == 1)
                    {
                        pixelValues[3 * i] = 255;
                        pixelValues[3 * i + 1] = 255;
                        pixelValues[3 * i + 2] = 255;
                    }
                    if(treshold == -1)
                    {
                        pixelValues[3 * i] = 0;
                        pixelValues[3 * i + 1] = 0;
                        pixelValues[3 * i + 2] = 0;
                    }
                    
                }               
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
