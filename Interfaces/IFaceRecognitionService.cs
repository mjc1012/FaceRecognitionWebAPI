using FaceRecognitionWebAPI.Face_Recognition_Model;
using FaceRecognitionWebAPI.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using OpenCvSharp;
using System.Drawing;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceRecognitionService
    {

        public Mat DetectFace(Bitmap img);
        public int RecognizeFace(FaceToRecognize face);

        public void TrainModel();
    }
}
