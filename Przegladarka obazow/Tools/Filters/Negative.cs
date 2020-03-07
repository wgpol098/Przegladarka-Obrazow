using Przegladarka_obazow.Tools.LUT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Filters
{
    public class Negative
    {
        int val = 0;
        public Negative(int val_)
        {
            val = val_;
        }

        public void ApplyInPlace(Bitmap bitmap)
        {
            int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            byte[] LUT = LutModifications.NegativeLUT();

            

            if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                int size = bitmap.Size.Width * bitmap.Size.Height * 3;

                if (val==0)
                    for (int index = 0; index < size; index++)
                    {
                        pixelValues[index] = LUT[pixelValues[index]];
                    }

                if (val == 1)
                    for (int index = 2; index < size; index += 3)
                    {
                        pixelValues[index] = LUT[pixelValues[index]];
                    }
                if(val == 2)
                    for (int index = 1; index < size; index += 3)
                    {
                        pixelValues[index] = LUT[pixelValues[index]];
                    }

                if(val == 3)
                    for (int index = 0; index < size; index += 3)
                    {
                        pixelValues[index] = LUT[pixelValues[index]];
                    }

            }

            /* Parallel.For(0, size, index =>
             {

                 pixelValues[index] = LUT[pixelValues[index]];

                 //if (val == 0 || val == 1) pixelValues[index + 2] = LUT[pixelValues[index + 2]];
                 //if (val == 0 || val == 2) pixelValues[index + 1] = LUT[pixelValues[index + 1]];
                 //if (val == 0 || val == 3) pixelValues[index] = LUT[pixelValues[index]];
             }
         );
         */
            /*
            //Parallel.For(0, height, y =>
            //{
            //Parallel.For(0, width, x =>
            //{
            Parallel.For(0, size1, index =>
             {

                 if (val == 0 || val == 1) pixelValues[index + 2] = LUT[pixelValues[index + 2]];
                 if (val == 0 || val == 2) pixelValues[index + 1] = LUT[pixelValues[index + 1]];
                 if (val == 0 || val == 3) pixelValues[index] = LUT[pixelValues[index]];
             }
            );
                 //   int index = y * bmpData.Stride + x * bytesPerPixel;
                    
                //});
            //});
            */

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
