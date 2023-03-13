using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceToRecognizeRepository
    {

        public Task<FaceToRecognize> GetFaceToRecognize(int id);
        public Task<FaceToRecognize> CreateFaceToRecognize(FaceToRecognize faceToRecognize);
    }
}
