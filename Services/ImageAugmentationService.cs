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
using AForge;
using AForge.Imaging.ColorReduction;
using AForge.Imaging.Filters;
using AForge.Math.Random;

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

                string path = Path.Combine(Path.Combine(_environment.ContentRootPath, "Face Dataset"), face.Person.Id.ToString() + "\\" + face.ImageFile);

                Bitmap image = new(path);


                image = SharpenImage(image);
                image = HistogramEqualizationColored(image);
                List<Bitmap> imageVariations1 = ImageFlip(image);

                foreach (Bitmap v1 in imageVariations1)
                {
                    List<Bitmap> imageVariations2 = ImageRotate(v1);

                    foreach (Bitmap v2 in imageVariations2)
                    {
                        List<Bitmap> imageVariations3 = ImageFilters(v2);

                        Random random = new();
                        List<int> randomNumbers = Enumerable.Range(0, 5).OrderBy(x => random.Next()).Take(5).ToList();

                        for (int i = 0; i < 5; i++)
                        {
                            await SaveImage(imageVariations3[i], face);
                            Bitmap v4 = imageVariations3[i];
                            switch (randomNumbers[i])
                            {
                                case 0:
                                    v4 = BlurImage(imageVariations3[i]);
                                    break;
                                case 1:
                                    v4 = RandomCutout(imageVariations3[i].ToMat());
                                    break;
                                case 2:
                                    v4 = AdditiveNoise(imageVariations3[i]);
                                    break;
                                case 3:
                                    v4 = ImageDistortion(imageVariations3[i]);
                                    break;
                                case 4:
                                    v4 = SaltAndPepper(imageVariations3[i]);
                                    break;
                            }
                            await SaveImage(v4, face);
                        }
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }





        public async Task SaveImage(Bitmap image, FaceToTrain face)
        {
            try
            {
                string imageFile = _imageService.SaveAugmentedImage(image, face.Person.Id);
                AugmentedFace augmentedFace = new()
                {
                    ImageFile = imageFile,
                    FaceToTrain = face
                };

                await _augmentedFaceRepository.CreateAugmentedFace(augmentedFace);
            }
            catch(Exception )
            {
                throw;
            }
            
        }

        public static Bitmap HistogramEqualizationColored(Bitmap Image)
        {
            try
            {
                HistogramEqualization filter = new();
                filter.ApplyInPlace(Image);
                return Image;

            }
            catch(Exception )
            {
                throw;
            }
        }

        public static Bitmap ImageDistortion(Bitmap image)
        {
            try
            {
                WaterWave filter = new()
                {
                    HorizontalWavesCount = 2,
                    HorizontalWavesAmplitude = 2,
                    VerticalWavesCount = 2,
                    VerticalWavesAmplitude = 2
                };
                return filter.Apply(image);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap SaltAndPepper(Bitmap image)
        {
            try
            {
                SaltAndPepperNoise filter = new(10);
                filter.ApplyInPlace(image);

                return image;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap RotateImage(Bitmap image, int angle)
        {
            try
            {
                RotateNearestNeighbor filter = new(angle, true);
                return filter.Apply(image);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Bitmap> ImageFlip(Bitmap image)
        {
            try
            {
                List<Bitmap> imageVariations = new() {
                { (Bitmap)image.Clone() },
                { HorizontalFlip((Bitmap)image.Clone()) },
                };
                return imageVariations;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Bitmap> ImageRotate(Bitmap image)
        {
            try
            {
                List<Bitmap> imageVariations = new() {
                { (Bitmap)image.Clone() },
                { RotateImage((Bitmap)image.Clone(),25) },
                { RotateImage((Bitmap)image.Clone(),-25) },
                };
                return imageVariations;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public static Bitmap ChangeBrightness(Bitmap image, int adjustValue)
        {
            try
            {
                Random random = new();
                if (random.Next(0, 2) == 1) adjustValue = -adjustValue;
                BrightnessCorrection brightnessCorrection = new(adjustValue);
                return brightnessCorrection.Apply(image);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap ChangeContrast(Bitmap image, int adjustValue)
        {
            try
            {
                Random random = new();
                if (random.Next(0, 2) == 1) adjustValue = -adjustValue;
                ContrastCorrection contrastCorrection = new(adjustValue);
                return contrastCorrection.Apply(image);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap ChangeSaturation(Bitmap image, float adjustValue)
        {
            try
            {
                Random random = new();
                if (random.Next(0, 2) == 1) adjustValue = -adjustValue;
                SaturationCorrection saturationCorrection = new(adjustValue / 100);
                return saturationCorrection.Apply(image);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap ChangeGamma(Bitmap image, double adjustValue)
        {
            try
            {
                GammaCorrection gammaCorrection = new(adjustValue / 100);
                gammaCorrection.ApplyInPlace(image);
                return image;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap SharpenImage(Bitmap image)
        {
            try
            {
                GaussianSharpen filter = new(4, 11);
                return filter.Apply(image);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<Bitmap> ImageFilters(Bitmap image)
        {
            try
            {
                Random random = new();
                List<Bitmap> imageVariations = new() {
                { (Bitmap)image.Clone() },
                { ChangeBrightness((Bitmap)image.Clone(), random.Next(40, 65)) },
                { ChangeContrast((Bitmap)image.Clone(), random.Next(40, 65)) },
                { ChangeSaturation((Bitmap)image.Clone(), random.Next(20, 35)) },
                { ChangeGamma((Bitmap)image.Clone(), random.Next(30, 50)) }
                };
                return imageVariations;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public static Bitmap RandomCutout(Mat image)
        {
            try
            {
                Random rand = new();
                int x = rand.Next(10, 154);
                int y = rand.Next(10, 154);
                Cv2.Rectangle(image, new OpenCvSharp.Point(x, y), new OpenCvSharp.Point(x + 70, y + 70), Scalar.Black, -1);
                return image.ToBitmap();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap AdditiveNoise(Bitmap image)
        {
            try
            {
                IRandomNumberGenerator generator = new UniformGenerator(new AForge.Range(-50, 50));
                AdditiveNoise filter = new(generator);
                filter.ApplyInPlace(image);

                return image;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap BlurImage(Bitmap image)
        {
            try
            {
                GaussianBlur filter = new(4, 11);
                return filter.Apply(image);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Bitmap HorizontalFlip(Bitmap image)
        {
            try
            {
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                return image;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
