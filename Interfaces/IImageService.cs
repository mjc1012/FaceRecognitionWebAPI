﻿using FaceRecognitionWebAPI.Models;
using System.Drawing;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IImageService
    {
        public string SaveImage(string base64String, int personId);
        public string SaveAugmentedImage(Bitmap image, int personId);
        public Task<bool> DeleteImage(FaceToTrain face, List<AugmentedFace> augmentedFaces);

        public bool DeleteImage(FaceToRecognize face);
        public string ImagePathToBase64(string path);
    }
}
