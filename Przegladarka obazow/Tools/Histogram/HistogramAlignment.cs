using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Filters
{
    public class HistogramAlignment
    {
        int[] red = new int[256];
        int[] green = new int[256];
        int[] blue = new int[256];

        public void ApplyInPlace(Bitmap bitmap)
        {
            HistogramCount(bitmap);

            int[] LUTred = calculateLUT(red, bitmap.Width * bitmap.Height);
            int[] LUTgreen = calculateLUT(green, bitmap.Width * bitmap.Height);
            int[] LUTblue = calculateLUT(blue, bitmap.Width * bitmap.Height);

            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

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
                    pixelValues[index + 2] = (byte)LUTblue[pixelValues[index + 2]];
                    pixelValues[index + 1] = (byte)LUTgreen[pixelValues[index + 1]];
                    pixelValues[index] = (byte)LUTred[pixelValues[index]];
                });
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }

        private void HistogramCount(Bitmap bitmap)
        {
            Array.Clear(red, 0, 256);
            Array.Clear(green, 0, 256);
            Array.Clear(blue, 0, 256);

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
                    red[pixelValues[index + 2]]++;
                    green[pixelValues[index + 1]]++;
                    blue[pixelValues[index]]++;
                });
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
        private int[] calculateLUT(int[] values, int size)
        {
            double minValue = 0;
            for(int i=0;i<255;i++)
            {
                if(values[i] != 0)
                {
                    minValue = values[i];
                    break;
                }
            }

            int[] result = new int[256];
            double sum = 0;
            for(int i=0;i<256;i++)
            {
                sum += values[i];
                result[i] = (int)(((sum - minValue) / (size - minValue)) * 255.0);
            }
            return result;
        }
    }
}
