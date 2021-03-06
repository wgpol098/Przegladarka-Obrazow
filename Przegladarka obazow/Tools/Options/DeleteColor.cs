﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Options
{
    public class DeleteColor
    {
        private int R;
        private int G;
        private int B;
        public DeleteColor(int _R, int _G, int _B)
        {
            R = _R;
            G = _G;
            B = _B;
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
                    if (R - 100 < pixelValues[index + 2] && R + 100 > pixelValues[index + 2])
                    {
                        if (G - 100 < pixelValues[index + 1] && G + 100 > pixelValues[index + 1])
                        {
                            if (B - 100 < pixelValues[index] && B + 100 > pixelValues[index])
                            {
                                byte value = (byte)(0.299 * pixelValues[index + 2] + 0.587 * pixelValues[index + 1] + 0.114 * pixelValues[index]);
                                pixelValues[index] = value;
                                pixelValues[index + 1] = value;
                                pixelValues[index + 2] = value;
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
