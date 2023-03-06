using FaceRecognitionWebAPI.Face_Recognition_Model;
using FaceRecognitionWebAPI.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using OpenCvSharp;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceRecognitionService
    {
        public int RecognizeFace(FaceToRecognize face);

        public bool TrainModel();
    }
}
