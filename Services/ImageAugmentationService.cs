using FaceRecognitionWebAPI.Interfaces;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Random = System.Random;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Repository;
using SkiaSharp;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Xml.Linq;
using static Tensorflow.tensorflow;
using Microsoft.Extensions.Hosting;

namespace FaceRecognitionWebAPI.Services
{
    public class ImageAugmentationService : IImageAugmentationService
    {
        private readonly IAugmentedFaceRepository _augmentedFaceRepository;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _environment;

        public ImageAugmentationService(IAugmentedFaceRepository augmentedFaceRepository, IImageService imageService, IWebHostEnvironment environment)
        { 
            _augmentedFaceRepository= augmentedFaceRepository;
            _imageService = imageService;
            _environment = environment;
        }

        public async Task RunImageAugmentation(FaceToTrain face)
        {
            try
            {

                var path = Path.Combine(Path.Combine(_environment.ContentRootPath, "Face Dataset"), face.Person.Id.ToString() + "\\" + face.ImageFile);

                var image = new Bitmap(path);


                image = SharpenImage(image);
                image = HistogramEqualization(image);
                var imageVariations1 = ImageFlip(image);

                foreach (Bitmap v1 in imageVariations1)
                {
                    var imageVariations2 = ImageRotate(v1);

                    foreach (Bitmap v2 in imageVariations2)
                    {
                        var imageVariations3 = ImageFilters(v2);

                        Random random = new Random();
                        var randomNumbers = Enumerable.Range(0, 5).OrderBy(x => random.Next()).Take(5).ToList();

                        for (int i = 0; i < 5; i++)
                        {
                            await SaveImage(imageVariations3[i], face);
                            Bitmap v4 = imageVariations3[randomNumbers[i]];
                            switch (i)
                            {
                                case 0:
                                    v4 = BlurImage(v4);
                                    break;
                                case 1:
                                    v4 = RandomCutout(v4.ToMat());
                                    break;
                                case 2:
                                    v4 = SaltAndPepper(v4);
                                    break;
                            }
                            if (i < 3) await SaveImage(v4, face);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public double[][] RGB2HSI(Bitmap image)
        {
            int w = image.Width;
            int h = image.Height;

            BitmapData image_data = image.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            int bytes = image_data.Stride * image_data.Height;
            byte[] buffer = new byte[bytes];
            Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            image.UnlockBits(image_data);

            double[][] result = new double[w * h][];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int pos = x * 3 + y * image_data.Stride;
                    int r_pos = x + y * w;
                    result[r_pos] = new double[3];
                    double b = (double)buffer[pos];
                    double g = (double)buffer[pos + 1];
                    double r = (double)buffer[pos + 2];

                    double[] pixel = { r, g, b };
                    double num = 0.5 * (2 * r - g - b);
                    double den = Math.Sqrt(Math.Pow(r - g, 2) + (r - b) * (g - b));

                    double hue = Math.Acos(num / (den));
                    if (b > g)
                    {
                        hue = 2 * Math.PI - hue;
                    }

                    num = pixel.Min();
                    den = r + g + b;

                    double saturation = 1d - 3d * num / den;
                    if (saturation == 0)
                    {
                        hue = 0;
                    }

                    double intensity = (r + g + b) / 3;

                    result[r_pos][0] = hue;
                    result[r_pos][1] = saturation;
                    result[r_pos][2] = intensity;
                }
            }
            //transpose
            double[][] result_t = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                result_t[i] = new double[w * h];
                for (int j = 0; j < w * h; j++)
                {
                    result_t[i][j] = result[j][i];
                }
            }

            return result_t;
        }
        public Bitmap HistogramEqualization(Bitmap image)
        {
            int w = image.Width;
            int h = image.Height;
            double[][] hsi = RGB2HSI(image);
            double[] intensities = hsi[2];
            //normalize
            double max = 0d;
            for (int i = 0; i < intensities.Length; i++)
            {
                max = Math.Max(max, intensities[i]);
            }
            for (int i = 0; i < intensities.Length; i++)
            {
                intensities[i] /= max;
            }
            //calculate new intensities
            double mean = 0d;
            while (Math.Round(mean, 2) != 0.5)
            {
                mean = intensities.Sum() / intensities.Length;
                double theta = Math.Log(0.5) / Math.Log(mean);
                for (int i = 0; i < intensities.Length; i++)
                {
                    intensities[i] = Math.Pow(intensities[i], theta);
                }
            }
            //normalize to range
            double min = 255d;
            max = 0d;
            for (int i = 0; i < intensities.Length; i++)
            {
                min = Math.Min(min, intensities[i]);
                max = Math.Max(max, intensities[i]);
            }
            for (int i = 0; i < intensities.Length; i++)
            {
                intensities[i] = 255 * (intensities[i] - min) / (max - min);
            }
            //make new image
            double[][] result = { hsi[0], hsi[1], intensities };
            Bitmap res_img = HSI2RGB(result, w, h);
            return res_img;
        }
        public Bitmap HSI2RGB(double[][] hsi_map, int w, int h)
        {

            Bitmap image = new Bitmap(w, h);
            BitmapData image_data = image.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            int bytes = image_data.Stride * image_data.Height;
            byte[] result = new byte[bytes];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int pos = x * 3 + y * image_data.Stride;
                    int hsi_pos = x + y * w;
                    double H = hsi_map[0][hsi_pos];
                    double S = hsi_map[1][hsi_pos];
                    double I = hsi_map[2][hsi_pos];
                    byte red = 0;
                    byte green = 0;
                    byte blue = 0;

                    if (H >= 0 && H < 2 * Math.PI / 3)
                    {
                        blue = (byte)(I * (1 - S));
                        red = (byte)(I * (1 + S * Math.Cos(H / Math.Cos(Math.PI / 3 - H))));
                        green = (byte)(3 * I - (red + blue));
                    }

                    else if (H >= 2 * Math.PI / 3 && H < 4 * Math.PI / 3)
                    {
                        red = (byte)(I * (1 - S));
                        green = (byte)(I * (1 + S * Math.Cos(H - 2 * Math.PI / 3) / Math.Cos(Math.PI - H)));
                        blue = (byte)(3 * I - (red + green));
                    }

                    else if (H >= 4 * Math.PI / 3 && H < 2 * Math.PI)
                    {
                        green = (byte)(I * (1 - S));
                        blue = (byte)(I * (1 + S * Math.Cos(H - 4 * Math.PI / 3) / Math.Cos(5 * Math.PI / 3 - H)));
                        red = (byte)(3 * I - (green + blue));
                    }

                    result[pos] = blue;
                    result[pos + 1] = green;
                    result[pos + 2] = red;
                }
            }
            Marshal.Copy(result, 0, image_data.Scan0, bytes);
            image.UnlockBits(image_data);
            return image;
        }

        

        public Bitmap SaltAndPepper(Bitmap image)
        {
            //int w = image.Width;
            //int h = image.Height;

            //BitmapData image_data = image.LockBits(
            //    new Rectangle(0, 0, w, h),
            //    ImageLockMode.ReadOnly,
            //    PixelFormat.Format24bppRgb);
            //int bytes = image_data.Stride * image_data.Height;
            //byte[] buffer = new byte[bytes];
            //byte[] result = new byte[bytes];
            //Marshal.Copy(image_data.Scan0, buffer, 0, bytes);
            //image.UnlockBits(image_data);

            //Random rnd = new Random();
            //int noise_chance = 10;
            //for (int i = 0; i < bytes; i += 3)
            //{
            //    int max = (int)(1000 / noise_chance);
            //    int tmp = rnd.Next(max + 1);
            //    for (int j = 0; j < 3; j++)
            //    {
            //        if (tmp == 0 || tmp == max)
            //        {
            //            int sorp = tmp / max;
            //            result[i + j] = (byte)(sorp * 255);
            //        }
            //        else
            //        {
            //            result[i + j] = buffer[i + j];
            //        }
            //    }
            //}

            //Bitmap result_image = new Bitmap(w, h);
            //BitmapData result_data = result_image.LockBits(
            //    new Rectangle(0, 0, w, h),
            //    ImageLockMode.WriteOnly,
            //    PixelFormat.Format24bppRgb);
            //Marshal.Copy(result, 0, result_data.Scan0, bytes);
            //result_image.UnlockBits(result_data);

            return image;
        }



        public Bitmap HorizontalFlip(Bitmap image)
        {
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return image;
        }

        public List<Bitmap> ImageFlip(Bitmap image)
        {
            var imageVariations = new List<Bitmap>() {
            { (Bitmap)image.Clone() },
            { HorizontalFlip((Bitmap)image.Clone()) },
            };
            return imageVariations;
        }

        public Bitmap RotateImage(Bitmap image, float angle)
        {
            Bitmap rotatedImage = new Bitmap(image.Width, image.Height);
            rotatedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(image.Width / 2, image.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-image.Width / 2, -image.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(image, new System.Drawing.Point(0, 0));
            }

            return rotatedImage;
        }

        public List<Bitmap> ImageRotate(Bitmap image)
        {

            var imageVariations = new List<Bitmap>() {
            { (Bitmap)image.Clone() },
            { RotateImage((Bitmap)image.Clone(),25) },
            { RotateImage((Bitmap)image.Clone(),-25) },
            };
            return imageVariations;
        }
        public Bitmap ChangeBrightness(Bitmap image, float value)
        {
            float b = value;
            // Make the ColorMatrix.
            ColorMatrix cm = new ColorMatrix(new float[][]
                {
                    new float[] {b, 0, 0, 0, 0},
                    new float[] {0, b, 0, 0, 0},
                    new float[] {0, 0, b, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1},
                });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            // Draw the image onto the new bitmap while applying the new ColorMatrix.
            System.Drawing.Point[] points =
            {
                new System.Drawing.Point(0, 0),
                new System.Drawing.Point(image.Width, 0),
                new System.Drawing.Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            // Make the result bitmap.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect, GraphicsUnit.Pixel, attributes);
            }

            // Return the result.
            return bm;
        }

        public Bitmap ChangeContrast(Bitmap image, int value)
        {
            BitmapData sourceData = image.LockBits(new Rectangle(0, 0,
                                        image.Width, image.Height),
                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            image.UnlockBits(sourceData);

            double contrastLevel = Math.Pow((100.0 + value) / 100.0, 2);


            double blue = 0;
            double green = 0;
            double red = 0;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = ((((pixelBuffer[k] / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0;


                green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0;


                red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) *
                            contrastLevel) + 0.5) * 255.0;


                if (blue > 255)
                { blue = 255; }
                else if (blue < 0)
                { blue = 0; }


                if (green > 255)
                { green = 255; }
                else if (green < 0)
                { green = 0; }


                if (red > 255)
                { red = 255; }
                else if (red < 0)
                { red = 0; }


                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }


            Bitmap resultBitmap = new Bitmap(image.Width, image.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                        resultBitmap.Width, resultBitmap.Height),
                                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }
 
        public List<Bitmap> ImageFilters(Bitmap image)
        {
            Random random = new Random();
            var imageVariations = new List<Bitmap>() {
            { (Bitmap)image.Clone() },
            { ChangeBrightness((Bitmap)image.Clone(), 0.5f) },
            { ChangeBrightness((Bitmap) image.Clone(), 1.5f) },
            { ChangeContrast((Bitmap)image.Clone(), -30) },
            { ChangeContrast((Bitmap)image.Clone(), 30) },
            };
            return imageVariations;
        }

        public Bitmap RandomCutout(Mat image)
        {
            Random rand = new Random();
            int x = rand.Next(10, 154);
            int y = rand.Next(10, 154);
            Cv2.Rectangle(image, new OpenCvSharp.Point(x, y), new OpenCvSharp.Point(x + 70, y + 70), Scalar.Black, -1);
            return image.ToBitmap();
        }

        public Bitmap SharpenImage(Bitmap image)
        {
            var sharpenImage = image.Clone() as Bitmap;

            int width = image.Width;
            int height = image.Height;

            // Create sharpening filter.
            const int filterWidth = 5;
            const int filterHeight = 5;

            var filter = new double[,]
                {
                    {-1, -1, -1, -1, -1},
                    {-1,  2,  2,  2, -1},
                    {-1,  2, 16,  2, -1},
                    {-1,  2,  2,  2, -1},
                    {-1, -1, -1, -1, -1}
                };

            var strength = 1;
            double bias = 1.0 - strength;
            double factor = strength / 16.0;

            var result = new Color[image.Width, image.Height];

            // Lock image bits for read/write.
            if (sharpenImage != null)
            {
                BitmapData pbits = sharpenImage.LockBits(new Rectangle(0, 0, width, height),
                                                            ImageLockMode.ReadWrite,
                                                            PixelFormat.Format24bppRgb);

                // Declare an array to hold the bytes of the bitmap.
                int bytes = pbits.Stride * height;
                var rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                Marshal.Copy(pbits.Scan0, rgbValues, 0, bytes);

                int rgb;
                // Fill the color array with the new sharpened color values.
                for (int x = 0; x < width; ++x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        double red = 0.0, green = 0.0, blue = 0.0;

                        for (int filterX = 0; filterX < filterWidth; filterX++)
                        {
                            for (int filterY = 0; filterY < filterHeight; filterY++)
                            {
                                int imageX = (x - filterWidth / 2 + filterX + width) % width;
                                int imageY = (y - filterHeight / 2 + filterY + height) % height;

                                rgb = imageY * pbits.Stride + 3 * imageX;

                                red += rgbValues[rgb + 2] * filter[filterX, filterY];
                                green += rgbValues[rgb + 1] * filter[filterX, filterY];
                                blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                            }

                            rgb = y * pbits.Stride + 3 * x;

                            int r = Math.Min(Math.Max((int)(factor * red + (bias * rgbValues[rgb + 2])), 0), 255);
                            int g = Math.Min(Math.Max((int)(factor * green + (bias * rgbValues[rgb + 1])), 0), 255);
                            int b = Math.Min(Math.Max((int)(factor * blue + (bias * rgbValues[rgb + 0])), 0), 255);

                            result[x, y] = Color.FromArgb(r, g, b);
                        }
                    }
                }

                // Update the image with the sharpened pixels.
                for (int x = 0; x < width; ++x)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        rgb = y * pbits.Stride + 3 * x;

                        rgbValues[rgb + 2] = result[x, y].R;
                        rgbValues[rgb + 1] = result[x, y].G;
                        rgbValues[rgb + 0] = result[x, y].B;
                    }
                }

                // Copy the RGB values back to the bitmap.
                Marshal.Copy(rgbValues, 0, pbits.Scan0, bytes);
                // Release image bits.
                sharpenImage.UnlockBits(pbits);
            }
                return sharpenImage;
        }

        public Bitmap BlurImage(Bitmap image)
        {
            Random random = new();
            var values = new[] { 2, 3, 4 };
            int blurSize = values[random.Next(values.Length)];
            Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the
                    // blur size while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return blurred;
        }



        public async Task SaveImage(Bitmap image, FaceToTrain face)
        {
            try
            {
                var imageFile = _imageService.SaveAugmentedImage(image, face.Person.Id);
                AugmentedFace augmentedFace = new AugmentedFace
                {
                    ImageFile = imageFile,
                    FaceToTrain = face
                };

                await _augmentedFaceRepository.CreateAugmentedFace(augmentedFace);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
