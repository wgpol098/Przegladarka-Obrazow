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
                int l = 0;
                byte value = (byte)pixelValues[3 * i];

                for(int y=0;y<256;y++)
                {
                    for(int z=0; z<256; z++)
                    {
                        int x = (int)Math.Round((value - 0.587 * y - 0.144 * z) / 0.299);
                        if (x < 256 && x >= 0)
                        {
                            int y1 = (int)Math.Round((value - 0.299 * x - 0.144 * z) / 0.587);
                            int z1 = (int)Math.Round((value - 0.299 * x - 0.587 * y1) / 0.144);
                            x = (int)Math.Round((value - 0.587 * y - 0.144 * z) / 0.299);
                            if (x < 256 && x >= 0)
                            {
                                y1 = (int)Math.Round((value - 0.299 * x - 0.144 * z) / 0.587);
                                z1 = (int)Math.Round((value - 0.299 * x - 0.587 * y1) / 0.144);
                                if (value == (byte)Math.Round(x * 0.299 + y1 * 0.587 + z1 * 0.114))
                                {
                                    pixelValues[3 * i] = (byte)x;
                                    pixelValues[3 * i + 1] = (byte)y1;
                                    pixelValues[3 * i + 2] = (byte)z1;
                                    l++;
                                    //MessageBox.Show(x + " " + y1 + " " + z1);
                                    break;
                                }
                            }
                        }
                        /*
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
                        */
                    }
                }
                //MessageBox.Show(l.ToString());
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
