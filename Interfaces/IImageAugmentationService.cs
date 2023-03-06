using FaceRecognitionWebAPI.Models;
using OpenCvSharp;
using System.Drawing;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IImageAugmentationService
    {
        public Task RunImageAugmentation(FaceToTrain face);
    }
}
