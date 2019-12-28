﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przegladarka_obazow.Tools.Modifications
{
    public class PixelsRange
    {
        int inPR = 0;
        int outPR = 0;

        bool R = false;
        bool G = false;
        bool B = false;

        public PixelsRange(int _inPR, int _outPR, bool _R, bool _G, bool _B)
        {
            inPR = _inPR;
            outPR = _outPR;
            R = _R;
            G = _G;
            B = _B;
        }
        public void ApplyInPlace(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            int size = bitmap.Width * bitmap.Height;
            Parallel.For(0, size, i =>
            {
                if(B==true)
                {
                    if (outPR <= pixelValues[3 * i]) pixelValues[3 * i] = (byte)outPR;
                    else if (inPR >= pixelValues[3 * i]) pixelValues[3 * i] = (byte)inPR;
                }
                
                if(G==true)
                {
                    if (outPR <= pixelValues[3 * i + 1]) pixelValues[3 * i + 1] = (byte)outPR;
                    else if (inPR >= pixelValues[3 * i + 1]) pixelValues[3 * i + 1] = (byte)inPR;
                }
                
                if(R==true)
                {
                    if (outPR <= pixelValues[3 * i + 2]) pixelValues[3 * i + 2] = (byte)outPR;
                    else if (inPR >= pixelValues[3 * i + 2]) pixelValues[3 * i + 2] = (byte)inPR;
                }
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}