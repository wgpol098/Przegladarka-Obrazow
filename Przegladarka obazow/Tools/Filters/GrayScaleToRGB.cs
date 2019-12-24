using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Przegladarka_obazow.Tools.Filters
{
    public class GrayScaleToRGB
    {
        public void ApplyInPlace(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            int size = bitmap.Width * bitmap.Height;
            Parallel.For(0, size, i =>
            {
                byte value = (byte)pixelValues[3 * i];

                for(byte x=0;x<255;x++)
                {
                    for(byte y=0; y<255;y++)
                    {
                        for(byte z=0;z<255;z++)
                        {
                            if(value == (byte) ((byte)(x*0.299) + (byte)(y*0.587) + (byte)(z*0.114)))
                            {
                                pixelValues[3 * i] = x;
                                pixelValues[3 * i + 1] = y;
                                pixelValues[3 * i + 2] = z;
                                break;
                            }
                        }
                    }
                }
               // MessageBox.Show("poszło");


                /*
                //byte value = (byte)(0.299 * pixelValues[3 * i + 2] + 0.587 * pixelValues[3 * i + 1] + 0.114 * pixelValues[3 * i]);
                pixelValues[3 * i] = value;
                pixelValues[3 * i + 1] = value;
                pixelValues[3 * i + 2] = value;
                */
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
