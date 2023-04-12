using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace FaceRecognitionWebAPI.Services
{
    public class ImageService : IImageService
    {

        private readonly IWebHostEnvironment _environment;
        private readonly IFaceRecognitionService _faceRecognitionService;

        public ImageService(IWebHostEnvironment environment, IFaceRecognitionService faceRecognitionService)
        {
            _environment = environment;
            _faceRecognitionService = faceRecognitionService;
        }

        public string SaveImage(string base64String, int personId)
        {
            try
            {
                string path = (personId == -1) ? Path.Combine(_environment.ContentRootPath, "Faces To Recognize") : Path.Combine(Path.Combine(_environment.ContentRootPath, "Face Dataset"), personId.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                
                Mat detectedFace = _faceRecognitionService.DetectFace((Bitmap)image);

                string fileWithPath;
                string newFileName;
                do
                {
                    string uniqueString = Guid.NewGuid().ToString();
                    newFileName = uniqueString + ".jpg";
                    fileWithPath = Path.Combine(path, newFileName);
                } while(Directory.Exists(fileWithPath));

                Mat imageToSave = new();
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
                string path = Path.Combine(Path.Combine(_environment.ContentRootPath, "Augmented Faces"), personId.ToString());


                string fileWithPath;
                string newFileName;
                do
                {
                    string uniqueString = Guid.NewGuid().ToString();
                    newFileName = uniqueString + ".jpg";
                    fileWithPath = Path.Combine(path, newFileName);
                } while (Directory.Exists(fileWithPath));

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
                string augmentedFacePath = Path.Combine(_environment.ContentRootPath, "Augmented Faces" + "\\" + face.PersonId.ToString());
                foreach (AugmentedFace augmentedFace in augmentedFaces)
                {
                    string tempPath = Path.Combine(augmentedFacePath, augmentedFace.ImageFile); 
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }

                if (Directory.Exists(augmentedFacePath) && !Directory.EnumerateFileSystemEntries(augmentedFacePath).Any())
                {
                    Directory.Delete(augmentedFacePath, true);
                }
                
                string faceToTrainPath = Path.Combine(_environment.ContentRootPath, "Face Dataset" + "\\" + face.PersonId.ToString());
                if (File.Exists(Path.Combine(faceToTrainPath, face.ImageFile)))
                {
                    File.Delete(Path.Combine(faceToTrainPath, face.ImageFile));

                    if (Directory.Exists(faceToTrainPath) && !Directory.EnumerateFileSystemEntries(faceToTrainPath).Any())
                    {
                        Directory.Delete(faceToTrainPath, true);
                    }
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

                string path = Path.Combine(_environment.ContentRootPath, "Faces To Recognize" + "\\" + face.ImageFile);
                if (File.Exists(path))
                {
                    File.Delete(path);
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
                string augmentedFacePath = Path.Combine(_environment.ContentRootPath, "Augmented Faces" + "\\" + id.ToString());

                string faceToTrainPath = Path.Combine(_environment.ContentRootPath, "Face Dataset" + "\\" + id.ToString());
                if (Directory.Exists(augmentedFacePath))
                {
                    Directory.Delete(augmentedFacePath, true);
                }

                if (Directory.Exists(faceToTrainPath))
                {
                    Directory.Delete(faceToTrainPath, true);
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
            using Image image = Image.FromFile(path);
            using MemoryStream m = new();
            image.Save(m, image.RawFormat);
            byte[] imageBytes = m.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            m.Close();
            return base64String;
        }
    }
}
