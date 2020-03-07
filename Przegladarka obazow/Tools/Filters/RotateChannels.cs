using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Filters
{
    public class RotateChannels
    {
        public void ApplyInPlace(Bitmap bitmap)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            if(bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                int size = bitmap.Size.Width * bitmap.Size.Height * 3;

                for (int index=0; index<size;index+=3)
                {
                    byte tmp = pixelValues[index + 2];
                    //R to G               
                    pixelValues[index + 2] = pixelValues[index + 1];
                    //G to B
                    pixelValues[index + 1] = pixelValues[index];
                    //B to R
                    pixelValues[index] = tmp;
                }
            }
            /*
            Parallel.For(0, height, y =>
            {
                Parallel.For(0, width, x =>
                {
                    int index = y * bmpData.Stride + x * bytesPerPixel;
                    byte tmp = pixelValues[index + 2];
                    //R to G               
                    pixelValues[index + 2] = pixelValues[index + 1];
                    //G to B
                    pixelValues[index + 1] = pixelValues[index];
                    //B to R
                    pixelValues[index] = tmp;
                });
            });
            */
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
