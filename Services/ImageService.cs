using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace FaceRecognitionWebAPI.Services
{
    public class ImageService : IImageService
    {

        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string SaveImage(string base64String, int personId)
        {
            try
            {
                var path = (personId == -1) ? Path.Combine(_environment.ContentRootPath, "Faces To Recognize") : Path.Combine(Path.Combine(_environment.ContentRootPath, "Face Dataset"), personId.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);

                Bitmap img = (Bitmap)image;

                Net faceNet = CvDnn.ReadNetFromCaffe("D:\\THESIS FINAL OUTPUT\\FaceRecognitionWebAPI\\Face Detection Model\\deploy.prototxt.txt", "D:\\THESIS FINAL OUTPUT\\FaceRecognitionWebAPI\\Face Detection Model\\res10_300x300_ssd_iter_140000_fp16.caffemodel");

                Mat newImage = img.ToMat();
                OpenCvSharp.Cv2.CvtColor(newImage, newImage, OpenCvSharp.ColorConversionCodes.RGBA2RGB);
                int frameHeight = newImage.Rows;
                int frameWidth = newImage.Cols;

                var blob = CvDnn.BlobFromImage(newImage, 1.0, new OpenCvSharp.Size(224, 224),
                    new Scalar(104, 117, 123), false, false);

                faceNet.SetInput(blob, "data");

                var detection = faceNet.Forward("detection_out");
                var detectionMat = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F,
                    detection.Ptr(0));

                float confidence = detectionMat.At<float>(0, 2);
                int x1 = (int)(detectionMat.At<float>(0, 3) * frameWidth);
                int y1 = (int)(detectionMat.At<float>(0, 4) * frameHeight);
                int x2 = (int)(detectionMat.At<float>(0, 5) * frameWidth);
                int y2 = (int)(detectionMat.At<float>(0, 6) * frameHeight);

                Rect roi = new(x1, y1, x2 - x1, y2 - y1);
                var detectedFace = newImage.Clone(roi);

                string uniqueString = Guid.NewGuid().ToString();
                // create a unique filename here
                var newFileName = uniqueString + ".jpg";
                var fileWithPath = Path.Combine(path, newFileName);
                Mat imageToSave = new();
                //Save detected face
                Cv2.Resize(detectedFace, imageToSave, new OpenCvSharp.Size(224, 224));
                imageToSave.SaveImage(fileWithPath);
                ms.Close();

                return newFileName;
            }
            catch (Exception )
            {
                throw ;
            }
        }

        public string SaveAugmentedImage(Bitmap image, int personId)
        {
            try
            {
                var path = Path.Combine(Path.Combine(_environment.ContentRootPath, "Augmented Faces"), personId.ToString());


                string uniqueString = Guid.NewGuid().ToString();
                // create a unique filename here
                var newFileName = uniqueString + ".jpg";
                var fileWithPath = Path.Combine(path, newFileName);

                Mat newImage = new();
                Cv2.Resize(image.ToMat(), newImage, new OpenCvSharp.Size(244, 244));



                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                newImage.SaveImage(fileWithPath);
                return newFileName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteImage(FaceToTrain face, List<AugmentedFace> augmentedFaces)
        {
            try
            {

                foreach(var augmentedFace in augmentedFaces)
                {
                    var augmentedFacePath = Path.Combine(_environment.ContentRootPath, "Augmented Faces" + "\\" + face.PersonId.ToString() + "\\" + augmentedFace.ImageFile); 
                    if (System.IO.File.Exists(augmentedFacePath))
                    {
                        System.IO.File.Delete(augmentedFacePath);
                    }
                }
                
                var faceToTrainPath = Path.Combine(_environment.ContentRootPath, "Face Dataset" + "\\" + face.PersonId.ToString()+ "\\"+ face.ImageFile);
                if (System.IO.File.Exists(faceToTrainPath))
                {
                    File.Delete(faceToTrainPath);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteImage(FaceToRecognize face)
        {
            try
            {

                var path = Path.Combine(_environment.ContentRootPath, "Faces To Recognize" + "\\" + face.ImageFile);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteFolder(int id)
        {
            try
            {
                var augmentedFacePath = Path.Combine(_environment.ContentRootPath, "Augmented Faces" + "\\" + id.ToString());

                var faceToTrainPath = Path.Combine(_environment.ContentRootPath, "Face Dataset" + "\\" + id.ToString());
                if (System.IO.File.Exists(augmentedFacePath))
                {
                    File.Delete(augmentedFacePath);
                }

                if (System.IO.File.Exists(faceToTrainPath))
                {
                    File.Delete(faceToTrainPath);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string ImagePathToBase64(string path)
        {
            using System.Drawing.Image image = System.Drawing.Image.FromFile(path);
            using MemoryStream m = new();
            image.Save(m, image.RawFormat);
            byte[] imageBytes = m.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            m.Close();
            return base64String;
        }
    }
}
