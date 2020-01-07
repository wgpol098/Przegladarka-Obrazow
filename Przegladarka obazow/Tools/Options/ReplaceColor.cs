using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Options
{
    public class ReplaceColor
    {
        private int R1;
        private int R2;
        private int G1;
        private int G2;
        private int B1;
        private int B2;

        public ReplaceColor(int _R1, int _G1, int _B1, int _R2, int _G2, int _B2)
        {
            R1 = _R1;
            R2 = _R2;
            G1 = _G1;
            G2 = _G2;
            B1 = _B1;
            B2 = _B2;
        }

        public void ApplyInPlace(Bitmap bitmap)
        {
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
                    if (R1 - 50 < pixelValues[index + 2] && R1 + 50 > pixelValues[index + 2])
                    {
                        if (G1 - 50 < pixelValues[index + 1] && G1 + 50 > pixelValues[index + 1])
                        {
                            if (B1 - 50 < pixelValues[index] && B1 + 50 > pixelValues[index])
                            {
                                pixelValues[index] = (byte)B2;
                                pixelValues[index + 1] = (byte)G2;
                                pixelValues[index + 2] = (byte)R2;
                            }
                        }
                    }
                });
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
