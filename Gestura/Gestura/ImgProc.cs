#region Library Files
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Xml;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading;
#endregion

namespace Gestura
{
    class ImgProc
    {
        #region Variable and Initializations
        //HMM parameters
        private static double[,] A = new double[8, 8];
        private static double[,] B = new double[8, 8];
        private static double[,] PI = new double[1, 8];

        public byte[] rgbValues;
        public static int[,] cntr = new int[50, 3];
        public int width = 128;
        public int height = 128;
        public static Image[] image = new Image[20];
        public Bitmap original;
        public Bitmap inputBitmap;
        private DataProcess dataProcess = new DataProcess();
        #endregion
        
        #region Initialization
        public ImgProc()
        {
            //Img_Processing(i);
        }
        
        //initialising the arrays for HMM data for processing

        public void Img_Processing(int i)
        {
            //Step_A(image[i]);
            if (i == 17)
                MessageBox.Show("Done");
        }
        #endregion

        #region Image Processing Functions
        public void Step_A(int i)
        {
            String temp = "D:\\Images\\frame";
            String num;
            num = i.ToString();
            String temp2 = ".jpg";
            temp2 = num + temp2;
            temp = temp + temp2;
            Image image;

            image = Image.FromFile(temp);
            original = new Bitmap(image, new Size(width, height));

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            System.Drawing.Imaging.BitmapData bmpData = original.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, original.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            int scan0 = ptr.ToInt32();

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * original.Height;
            rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            rgbValues = RGBtoHSV(original, rgbValues, scan0, bmpData.Stride);
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            // Unlock the bits.
            original.UnlockBits(bmpData);
            
            temp2 = ".bmp";
            temp = "D:\\images2\\frame";
            temp2 = num + temp2;
            temp = temp + temp2;
            original.Save(temp);
            original.Dispose(); 
        }
 
        public void Step_B(int i,ref Bitmap temp)
        {
            inputBitmap = temp;
            Bitmap outBitmap = LoG12x12(inputBitmap);
        }

        public void Step_C(int i,ref Bitmap bit_img)
        {
            Bitmap original = new Bitmap(bit_img, new Size(width, height));

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            System.Drawing.Imaging.BitmapData bmpData = original.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, original.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            int scan0 = ptr.ToInt32();

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * original.Height;
            rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            centroid(original, ref rgbValues, scan0, bmpData.Stride, ref cntr[i, 0], ref cntr[i, 1], ref cntr[i, 2]);
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            original.UnlockBits(bmpData);
        }
         
        public byte[] RGBtoHSV(Bitmap original, byte[] rgbvalues, int scan0, int stride)
        {

            //make an empty bitmap the same size as original
            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    double originalColorR = rgbvalues[(j * stride) + (i * 4) + 2];
                    double originalColorG = rgbvalues[(j * stride) + (i * 4) + 1];
                    double originalColorB = rgbvalues[(j * stride) + (i * 4)];

                    double max = Math.Max(originalColorR, originalColorG);
                    max = Math.Max(originalColorB, max);
                    double min = Math.Min(originalColorR, originalColorG);
                    min = Math.Min(originalColorB, min);

                    double s = (max == 0) ? 0.0 : (1.0 - (min / max));
                    double v = max;
                    double h;

                    double r1 = (max - originalColorR) / (max - min);
                    double g1 = (max - originalColorG) / (max - min);
                    double b1 = (max - originalColorB) / (max - min);

                    if (originalColorR == max && originalColorG == min)
                        h = 5 + b1;
                    else if (originalColorR == max && originalColorG >= min)
                        h = 1 - g1;
                    else if (originalColorG == max && originalColorB == min)
                        h = r1 + 1;
                    else if (originalColorG == max && originalColorB >= min)
                        h = 3 - b1;
                    else if (originalColorR == max)
                        h = 3 + g1;
                    else
                        h = 5 - r1;

                    h = h * 60;

                    // Skin Detection 
                    if ((h >= 0 && h <= 50) && (s >= 0.32 && s <= 0.57))
                    {
                        h = 255;
                        s = 255;
                        v = 255;
                    }
                    else
                    {
                        h = s = v = 0;
                    }

                    //create the color object
                    Color newColor = Color.FromArgb((int)(Math.Round(h, MidpointRounding.AwayFromZero)), (int)(Math.Round(s, MidpointRounding.AwayFromZero)), (int)(Math.Round(v, MidpointRounding.AwayFromZero)));

                    //set the new image's pixel to the grayscale version
                    rgbvalues[(j * stride) + (i * 4) + 2] = newColor.R;
                    rgbvalues[(j * stride) + (i * 4) + 1] = newColor.G;
                    rgbvalues[(j * stride) + (i * 4)] = newColor.B;
                }
            }
            return rgbvalues;
        }

        public static Bitmap LoG12x12(Bitmap SrcImage)
        {
            double[,] MASK = new double[12, 12] {
								{-0.000699762,	-0.000817119,	-0.000899703,	-0.000929447,	-0.000917118,	-0.000896245,	-0.000896245,	-0.000917118,	-0.000929447,	-0.000899703,	-0.000817119,	-0.000699762},
								{-0.000817119,	-0.000914231,	-0.000917118,	-0.000813449,	-0.000655442,	-0.000538547,	-0.000538547,	-0.000655442,	-0.000813449,	-0.000917118,	-0.000914231,	-0.000817119},
								{-0.000899703,	-0.000917118,	-0.000745635,	-0.000389918,	0.0000268,	0.000309618,	0.000309618,	0.0000268,	-0.000389918,	-0.000745635,	-0.000917118,	-0.000899703},
								{-0.000929447,	-0.000813449,	-0.000389918,	0.000309618,	0.001069552,	0.00156934,	0.00156934,	0.001069552,	0.000309618,	-0.000389918,	-0.000813449,	-0.000929447},
								{-0.000917118,	-0.000655442,	0.0000268,	0.001069552,	0.002167033,	0.002878738,	0.002878738,	0.002167033,	0.001069552,	0.0000268,	-0.000655442,	-0.000917118},
								{-0.000896245,	-0.000538547,	0.000309618,	0.00156934,	0.002878738,	0.003722998,	0.003722998,	0.002878738,	0.00156934,	0.000309618,	-0.000538547,	-0.000896245},
								{-0.000896245,	-0.000538547,	0.000309618,	0.00156934,	0.002878738,	0.003722998,	0.003722998,	0.002878738,	0.00156934,	0.000309618,	-0.000538547,	-0.000896245},
								{-0.000917118,	-0.000655442,	0.0000268,	0.001069552,	0.002167033,	0.002878738,	0.002878738,	0.002167033,	0.001069552,	0.0000268,	-0.000655442,	-0.000917118},
								{-0.000929447,	-0.000813449,	-0.000389918,	0.000309618,	0.001069552,	0.00156934,	0.00156934,	0.001069552,	0.000309618,	-0.000389918,	-0.000813449,	-0.000929447},
								{-0.000899703,	-0.000917118,	-0.000745635,	-0.000389918,	0.0000268,	0.000309618,	0.000309618,	0.0000268,	-0.000389918,	-0.000745635,	-0.000917118,	-0.000899703},
								{-0.000817119,	-0.000914231,	-0.000917118,	-0.000813449,	-0.000655442,	-0.000538547,	-0.000538547,	-0.000655442,	-0.000813449,	-0.000917118,	-0.000914231,	-0.000817119},
								{-0.000699762,	-0.000817119,	-0.000899703,	-0.000929447,	-0.000917118,	-0.000896245,	-0.000896245,	-0.000917118,	-0.000929447,	-0.000899703,	-0.000817119,	-0.000699762}
							};

            double nTemp = 0.0;
            double c = 0;

            int mdl, size;
            size = 12;
            mdl = size / 2;

            double sum = 0.0;
            double mean;
            double d = 0.0;
            double s = 0.0;
            int n = 0;

            Bitmap bitmap = new Bitmap(SrcImage.Width + mdl, SrcImage.Height + mdl);
            int l, k;

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData srcData = SrcImage.LockBits(new Rectangle(0, 0, SrcImage.Width, SrcImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            // compile with: /unsafe
            unsafe
            {
                double min,max ;
                min  = max = 0.0;
                int offset = 3;

                for (int colm = 0; colm < srcData.Height - size; colm++)
                {
                    byte* ptr = (byte*)srcData.Scan0 + (colm * srcData.Stride);
                    byte* bitmapPtr = (byte*)bitmapData.Scan0 + (colm * bitmapData.Stride);

                    for (int row = 0; row < srcData.Width - size; row++)
                    {
                        nTemp = 0.0;

                        min = double.MaxValue;
                        max = double.MinValue;

                        for (k = 0; k < size; k++)
                        {
                            for (l = 0; l < size; l++)
                            {
                                byte* tempPtr = (byte*)srcData.Scan0 + ((colm + l) * srcData.Stride);
                                c = (tempPtr[((row + k) * offset)] + tempPtr[((row + k) * offset) + 1] + tempPtr[((row + k) * offset) + 2]) / 3;

                                nTemp += (double)c * MASK[k, l];

                            }
                        }

                        sum += nTemp;
                        n++;
                    }
                }
                mean = ((double)sum / n);
                d = 0.0;

                for (int i = 0; i < srcData.Height - size; i++)
                {
                    byte* ptr = (byte*)srcData.Scan0 + (i * srcData.Stride);
                    byte* tptr = (byte*)bitmapData.Scan0 + (i * bitmapData.Stride);

                    for (int j = 0; j < srcData.Width - size; j++)
                    {
                        nTemp = 0.0;

                        min = double.MaxValue;
                        max = double.MinValue;

                        for (k = 0; k < size; k++)
                        {
                            for (l = 0; l < size; l++)
                            {
                                byte* tempPtr = (byte*)srcData.Scan0 + ((i + l) * srcData.Stride);
                                c = (tempPtr[((j + k) * offset)] + tempPtr[((j + k) * offset) + 1] + tempPtr[((j + k) * offset) + 2]) / 3;

                                nTemp += (double)c * MASK[k, l];

                            }
                        }

                        s = (mean - nTemp);
                        d += (s * s);
                    }
                }


                d = d / (n - 1);
                d = Math.Sqrt(d);
                d = d * 2;

                for (int colm = mdl; colm < srcData.Height - mdl; colm++)
                {
                    byte* ptr = (byte*)srcData.Scan0 + (colm * srcData.Stride);
                    byte* bitmapPtr = (byte*)bitmapData.Scan0 + (colm * bitmapData.Stride);

                    for (int row = mdl; row < srcData.Width - mdl; row++)
                    {
                        nTemp = 0.0;

                        min = double.MaxValue;
                        max = double.MinValue;

                        for (k = (mdl * -1); k < mdl; k++)
                        {
                            for (l = (mdl * -1); l < mdl; l++)
                            {
                                byte* tempPtr = (byte*)srcData.Scan0 + ((colm + l) * srcData.Stride);
                                c = (tempPtr[((row + k) * offset)] + tempPtr[((row + k) * offset) + 1] + tempPtr[((row + k) * offset) + 2]) / 3;
                                nTemp += (double)c * MASK[mdl + k, mdl + l];
                            }
                        }

                        if (nTemp > d)
                        {
                            bitmapPtr[row * offset] = bitmapPtr[row * offset + 1] = bitmapPtr[row * offset + 2] = 255;
                        }
                        else
                            bitmapPtr[row * offset] = bitmapPtr[row * offset + 1] = bitmapPtr[row * offset + 2] = 0;

                    }
                }
            }

            bitmap.UnlockBits(bitmapData);
            SrcImage.UnlockBits(srcData);

            return bitmap;
        }
        #endregion

        #region Feature Extraction(Centroid)
        public void centroid(Bitmap image, ref byte[] rgbvalues, int scan0, int stride, ref int x, ref int y, ref int quad)
        {
            int white_cnt, wc, best_i = 0, best_j = 0, best_cnt = 0;
            Color newColor2;
            int i, j, k, p, r, s;
            double originalColorR, originalColorG, originalColorB;

            for (i = 0; i + 45 < image.Width; i += 5)
                for (j = 0; j + 60 < image.Height; j += 5)
                {
                    white_cnt = wc = 0;
                    for (k = i; k < i + 45; k++)
                        for (p = j; p < j + 60; p++)
                        {
                            //get the pixel from the original image
                            originalColorR = rgbvalues[(p * stride) + (k * 4) + 2];
                            originalColorG = rgbvalues[(p * stride) + (k * 4) + 1];
                            originalColorB = rgbvalues[(p * stride) + (k * 4)];
                            if (originalColorR == 255 && originalColorG == 255 && originalColorB == 255)
                                white_cnt++;
                        }

                    if (white_cnt >= 100)
                    {
                        for (r = i; r < i + 45; r++)
                            for (s = j; s > j - 20 && s > 0; s--)
                            {
                                originalColorR = rgbvalues[(s * stride) + (r * 4) + 2];
                                originalColorG = rgbvalues[(s * stride) + (r * 4) + 1];
                                originalColorB = rgbvalues[(s * stride) + (r * 4)];
                                if (originalColorR == 255 && originalColorG == 255 && originalColorB == 255)
                                    wc++;
                            }

                        if (wc <= 40)
                        {
                            if (best_cnt == 0)
                                best_cnt = white_cnt;
                            if (best_cnt <= white_cnt)
                            {
                                best_i = i;
                                best_j = j;
                                best_cnt = white_cnt;
                            }
                        }
                    }
                }

            newColor2 = Color.FromArgb(255, 0, 0);
            for (k = best_i; k < best_i + 45; k++)
            {
                try
                {
                    rgbvalues[(best_j * stride) + (k * 4) + 2] = newColor2.R;
                    rgbvalues[(best_j * stride) + (k * 4) + 1] = newColor2.G;
                    rgbvalues[(best_j * stride) + (k * 4)] = newColor2.B;

                    rgbvalues[((best_j + 60) * stride) + (k * 4) + 2] = newColor2.R;
                    rgbvalues[((best_j + 60) * stride) + (k * 4) + 1] = newColor2.G;
                    rgbvalues[((best_j + 60) * stride) + (k * 4)] = newColor2.B;
                }
                catch (Exception er)
                {
                    MessageBox.Show("Error occured" + er.ToString());
                }
            }
            for (p = best_j; p < best_j + 60; p++)
            {
                try
                {
                    rgbvalues[(p * stride) + (best_i * 4) + 2] = 255;
                    rgbvalues[(p * stride) + (best_i * 4) + 1] = 0;
                    rgbvalues[(p * stride) + (best_i * 4)] = 0;

                    rgbvalues[(p * stride) + ((best_i + 45) * 4) + 2] = 255;
                    rgbvalues[(p * stride) + ((best_i + 45) * 4) + 1] = 0;
                    rgbvalues[(p * stride) + ((best_i + 45) * 4)] = 0;
                }
                catch (Exception er)
                {
                    MessageBox.Show("Error occured" + er.ToString());
                }
            }

            // render everything else outside the rectangle black
            x = best_i + 22;
            y = best_j + 30;

            // To find the quadrant
            decimal alpha = Math.Abs((image.Height - y) / (image.Width - x));
            if (x <= image.Width / 2) // quad 1,2,8,7
            {
                if (y <= image.Height / 2) //quad 1 or 2
                {
                    if (alpha < 1)
                        quad = 1;
                    else
                        quad = 2;
                }
                else  //quad 7 or 8
                {
                    if (alpha < 1)
                        quad = 8;
                    else
                        quad = 7;
                }
            }
            else    // quad 3,4,5,6
            {
                if (y <= image.Height / 2) //quad 3 or 4
                {
                    if (alpha < 1)
                        quad = 4;
                    else
                        quad = 3;
                }
                else  //quad 5 or 6
                {
                    if (alpha < 1)
                        quad = 5;
                    else
                        quad = 6;
                }
            }
        }
        #endregion

        #region Utility Function
        public int[,] ret_cntr_array()
        {
            return cntr;
        }
#endregion
    }
}