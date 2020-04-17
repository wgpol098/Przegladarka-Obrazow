using Przegladarka_obazow.Tools.LUT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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
            //byte[] pixelValues = new byte[((bmpData.Stride + (bmpData.Stride >> 31)) ^ (bmpData.Stride >> 31)) * bitmap.Height];
            //System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelValues, 0, pixelValues.Length);

            byte[] LUT = LutModifications.NegativeLUT();
            int size = bitmap.Size.Width * bitmap.Size.Height * bytesPerPixel;
            unsafe
            {
                byte* pixelValues = (byte*)bmpData.Scan0;
                if (val == 0)
                {
                    
                    int taskCount = 5;
                    Task[] task = new Task[taskCount];
                    /*for (int tk=0;tk<task.Length;tk++)
                    {
                        task[tk] = Task.Factory.StartNew(() =>
                        {
                                for (int index = size/taskCount * tk;  index < size/taskCount*(tk+1); index++)
                                {
                                    pixelValues[index] = LUT[pixelValues[index]];
                                }
                        });
                    }
                    */

                    task[0] = Task.Factory.StartNew(() =>
                    {
                        for (int index = size/5*0; index < size/5*1; index++)
                        {
                            pixelValues[index] = LUT[pixelValues[index]];
                            //pixelValues[index] = LUT[pixelValues[index]];
                        }
                    });
                    task[1] = Task.Factory.StartNew(() =>
                    {
                        for (int index = size/5; index < size/5*2; index++)
                        {
                            pixelValues[index] = LUT[pixelValues[index]];
                            //pixelValues[index] = LUT[pixelValues[index]];
                        }
                    });
                    task[2] = Task.Factory.StartNew(() =>
                    {
                        for (int index = size / 5*2; index < size / 5 * 3; index++)
                        {
                            pixelValues[index] = LUT[pixelValues[index]];
                            //pixelValues[index] = LUT[pixelValues[index]];
                        }
                    });
                    task[3] = Task.Factory.StartNew(() =>
                    {
                        for (int index = size / 5*3; index < size/5*4; index++)
                        {
                            pixelValues[index] = LUT[pixelValues[index]];
                            //pixelValues[index] = LUT[pixelValues[index]];
                        }
                    });
                    task[4] = Task.Factory.StartNew(() =>
                    {
                        for (int index = size / 5 * 4; index < size/5*5; index++)
                        {
                            pixelValues[index] = LUT[pixelValues[index]];
                            //pixelValues[index] = LUT[pixelValues[index]];
                        }
                    });
                    
                    Task.WaitAll(task);
                    
                }
            }
            
            //Vector<byte> LUT = LutModifications.NegativeLUT();

                          //else if (val == 1)
                              //for (int index = bytesPerPixel + 2; index < size; index += bytesPerPixel)
                              {
                                  //pixelValues[index] = LUT[pixelValues[index]];
                              }
                          //else if(val == 2)
                              //for (int index = bytesPerPixel + 1; index < size; index += bytesPerPixel)
                              {
                                  //pixelValues[index] = LUT[pixelValues[index]];
                              }

                          //else if(val == 3)
                              //for (int index = bytesPerPixel + 0; index < size; index += bytesPerPixel)
                              {
                                  //pixelValues[index] = LUT[pixelValues[index]];
                              }

            //System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            bitmap.UnlockBits(bmpData);
        }
    }
}
