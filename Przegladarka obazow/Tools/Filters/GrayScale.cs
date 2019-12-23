using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Filters
{
    public class GrayScale
    {
        public void ApplyInPlace(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            int size = bitmap.Width * bitmap.Height;
            Parallel.For(0, size, i =>
            {
                byte value = (byte)(0.299 * pixelValues[3 * i + 2] + 0.587 * pixelValues[3 * i + 1] + 0.114 * pixelValues[3 * i]);
                pixelValues[3 * i] = value;
                pixelValues[3 * i + 1] = value;
                pixelValues[3 * i + 2] = value;
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
