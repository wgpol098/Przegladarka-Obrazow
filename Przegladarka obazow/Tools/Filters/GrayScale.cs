using Accord.Imaging;
using Emgu.CV.Cuda;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Przegladarka_obazow.Tools.Filters
{
    public class GrayScale
    {
        public void ApplyInPlace(Bitmap bitmap)
        {
            
            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] pixelValues = new byte[Math.Abs(bmpData.Stride) * bitmap.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            if(bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                int size = bitmap.Size.Height * bitmap.Size.Width * 3;
                

                int taskCount = 4;
                Task[] task = new Task[taskCount];
                
                task[0] = Task.Factory.StartNew(() =>
                {
                    for (int index = size/taskCount*0; index < size/taskCount*1; index += bytesPerPixel)
                    {
                        byte value = (byte)(0.299 * pixelValues[index + 2] + 0.587 * pixelValues[index + 1] + 0.114 * pixelValues[index]);
                        pixelValues[index] = value;
                        pixelValues[index + 1] = value;
                        pixelValues[index + 2] = value;
                    }
                });
                task[1] = Task.Factory.StartNew(() =>
                {
                    for (int index = size / taskCount * 1; index < size / taskCount * 2; index += bytesPerPixel)
                    {
                        byte value = (byte)(0.299 * pixelValues[index + 2] + 0.587 * pixelValues[index + 1] + 0.114 * pixelValues[index]);
                        pixelValues[index] = value;
                        pixelValues[index + 1] = value;
                        pixelValues[index + 2] = value;
                    }
                });
                task[2] = Task.Factory.StartNew(() =>
                {
                    for (int index = size / taskCount * 2; index < size / taskCount * 3; index += bytesPerPixel)
                    {
                        byte value = (byte)(0.299 * pixelValues[index + 2] + 0.587 * pixelValues[index + 1] + 0.114 * pixelValues[index]);
                        pixelValues[index] = value;
                        pixelValues[index + 1] = value;
                        pixelValues[index + 2] = value;
                    }
                });
                task[3] = Task.Factory.StartNew(() =>
                {
                    for (int index = size / taskCount * 3; index < size / taskCount * 4; index += bytesPerPixel)
                    {
                        byte value = (byte)(0.299 * pixelValues[index + 2] + 0.587 * pixelValues[index + 1] + 0.114 * pixelValues[index]);
                        pixelValues[index] = value;
                        pixelValues[index + 1] = value;
                        pixelValues[index + 2] = value;
                    }
                });
                Task.WaitAll(task);
            }
            /*
            Parallel.For(0, height, y =>
            {
                Parallel.For(0, width, x =>
                {
                    int index = y * bmpData.Stride + x * bytesPerPixel;
                    byte value = (byte)(0.299 * pixelValues[index + 2] + 0.587 * pixelValues[index + 1] + 0.114 * pixelValues[index]);
                    pixelValues[index] = value;
                    pixelValues[index + 1] = value;
                    pixelValues[index + 2] = value;
                });
            });*/
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
