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

                var path = Path.Combine(Path.Combine(_environment.ContentRootPath, "Face Dataset"), face.Person.Id.ToString() + "\\" + face.ImageFile);

                var image = new Bitmap(path);


                image = sharpenImage(image);
                image = histogramEqualizationColored(image);
                var imageVariations1 = imageFlip(image);

                foreach (Bitmap v1 in imageVariations1)
                {
                    var imageVariations2 = imageRotate(v1);

                    foreach (Bitmap v2 in imageVariations2)
                    {
                        var imageVariations3 = imageFilters(v2);

                        Random random = new Random();
                        var randomNumbers = Enumerable.Range(0, 5).OrderBy(x => random.Next()).Take(5).ToList();

                        for (int i = 0; i < 5; i++)
                        {
                            await SaveImage(imageVariations3[i], face);
                            Bitmap v4 = imageVariations3[i];
                            switch (randomNumbers[i])
                            {
                                case 0:
                                    v4 = blurImage(imageVariations3[i]);
                                    break;
                                case 1:
                                    v4 = randomCutout(imageVariations3[i].ToMat());
                                    break;
                                case 2:
                                    v4 = additiveNoise(imageVariations3[i]);
                                    break;
                                case 3:
                                    v4 = imageDistortion(imageVariations3[i]);
                                    break;
                                case 4:
                                    v4 = saltAndPepper(imageVariations3[i]);
                                    break;
                            }
                            await SaveImage(v4, face);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
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

        public static Bitmap histogramEqualizationColored(Bitmap Image)
        {
            HistogramEqualization filter = new HistogramEqualization();
            filter.ApplyInPlace(Image);
            return Image;
        }

        public static Bitmap imageDistortion(Bitmap image)
        {
            WaterWave filter = new WaterWave();
            filter.HorizontalWavesCount = 2;
            filter.HorizontalWavesAmplitude = 2;
            filter.VerticalWavesCount = 2;
            filter.VerticalWavesAmplitude = 2;
            return filter.Apply(image);
        }

        public static Bitmap saltAndPepper(Bitmap image)
        {
            SaltAndPepperNoise filter = new SaltAndPepperNoise(10);
            filter.ApplyInPlace(image);

            return image;
        }

        public static Bitmap rotateImage(Bitmap image, int angle)
        {
            RotateNearestNeighbor filter = new RotateNearestNeighbor(angle, true);
            return filter.Apply(image);
        }

        public static List<Bitmap> imageFlip(Bitmap image)
        {
            Random random = new Random();

            var imageVariations = new List<Bitmap>() {
            { (Bitmap)image.Clone() },
            { horizontalFlip((Bitmap)image.Clone()) },
            };
            return imageVariations;
        }

        public static List<Bitmap> imageRotate(Bitmap image)
        {
            Random random = new Random();

            var imageVariations = new List<Bitmap>() {
            { (Bitmap)image.Clone() },
            { rotateImage((Bitmap)image.Clone(),25) },
            { rotateImage((Bitmap)image.Clone(),-25) },
            };
            return imageVariations;
        }


        public static Bitmap changeBrightness(Bitmap image, int adjustValue)
        {
            Random random = new Random();
            if (random.Next(0, 2) == 1) adjustValue = -adjustValue;
            BrightnessCorrection brightnessCorrection = new BrightnessCorrection(adjustValue);
            return brightnessCorrection.Apply(image);
        }

        public static Bitmap changeContrast(Bitmap image, int adjustValue)
        {
            Random random = new Random();
            if (random.Next(0, 2) == 1) adjustValue = -adjustValue;
            ContrastCorrection contrastCorrection = new ContrastCorrection(adjustValue);
            return contrastCorrection.Apply(image);
        }

        public static Bitmap changeSaturation(Bitmap image, float adjustValue)
        {
            Random random = new Random();
            if (random.Next(0, 2) == 1) adjustValue = -adjustValue;
            SaturationCorrection saturationCorrection = new SaturationCorrection(adjustValue / 100);
            return saturationCorrection.Apply(image);
        }

        public static Bitmap changeGamma(Bitmap image, double adjustValue)
        {
            GammaCorrection gammaCorrection = new GammaCorrection(adjustValue / 100);
            gammaCorrection.ApplyInPlace(image);
            return image;
        }

        public static Bitmap sharpenImage(Bitmap image)
        {
            GaussianSharpen filter = new GaussianSharpen(4, 11);
            return filter.Apply(image);
        }
        public static List<Bitmap> imageFilters(Bitmap image)
        {
            Random random = new Random();
            var imageVariations = new List<Bitmap>() {
            { (Bitmap)image.Clone() },
            { changeBrightness((Bitmap)image.Clone(), random.Next(40, 65)) },
            { changeContrast((Bitmap)image.Clone(), random.Next(40, 65)) },
            { changeSaturation((Bitmap)image.Clone(), random.Next(20, 35)) },
            { changeGamma((Bitmap)image.Clone(), random.Next(30, 50)) }
            };
            return imageVariations;
        }


        public static Bitmap randomCutout(Mat image)
        {
            Random rand = new Random();
            int x = rand.Next(10, 154);
            int y = rand.Next(10, 154);
            Cv2.Rectangle(image, new OpenCvSharp.Point(x, y), new OpenCvSharp.Point(x + 70, y + 70), Scalar.Black, -1);
            return image.ToBitmap();
        }

        public static Bitmap additiveNoise(Bitmap image)
        {
            Random random = new Random();
            IRandomNumberGenerator generator = new UniformGenerator(new AForge.Range(-50, 50));
            AdditiveNoise filter = new AdditiveNoise(generator);
            filter.ApplyInPlace(image);

            return image;
        }

        public static Bitmap blurImage(Bitmap image)
        {
            GaussianBlur filter = new GaussianBlur(4, 11);
            return filter.Apply(image);
        }

        public static Bitmap horizontalFlip(Bitmap image)
        {
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return image;
        }
    }
}
