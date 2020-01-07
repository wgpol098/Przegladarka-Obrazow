using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Przegladarka_obazow.Tools.LUT
{
    class LutModifications
    {
        public static void SetLUTToBitmap(Bitmap bitmap,byte[] LUT)
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
        }

        public static byte[] LightLUT(int lightvalue)
        {
            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
                if ((lightvalue + i) > 255) LUT[i] = 255;
                else if ((lightvalue + i) < 0) LUT[i] = 0;
                else LUT[i] = (byte)(lightvalue + i);

            return LUT;
        }

        public static byte[] GammaLUT(double gammavalue)
        {
            byte[] LUT = new byte[256];
            for (int i = 0; i < 256; i++)
                if ((255 * Math.Pow(i / 255.0, 1 / gammavalue)) > 255) LUT[i] = 255;
                else LUT[i] = (byte)(255 * Math.Pow(i / 255.0, 1 / gammavalue));

            return LUT;
        }

        public static byte[] CorrectionLUT(double correctionvalue)
        {        
            byte[] LUT = new byte[256];
            double a;

            if (correctionvalue <= 0) a = 1.0 + (correctionvalue / 256.0);
            else a = 256.0 / Math.Pow(2, Math.Log(257 - correctionvalue, 2));
            for (int i = 0; i < 256; i++)
                if ((a * (i - 127) + 127) > 255) LUT[i] = 255;
                else if ((a * (i - 127) + 127) < 0) LUT[i] = 0;
                else LUT[i] = (byte)(a * (i - 127) + 127);

            return LUT;
        }

        public static byte[] NegativeLUT()
        {
            byte[] LUT = new byte[256];
            for(int i=0; i < 256;i++) LUT[i] = (byte)(255 - i);
            return LUT;
        }
    }
}
