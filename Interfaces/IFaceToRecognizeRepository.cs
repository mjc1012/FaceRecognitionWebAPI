using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceToRecognizeRepository
    {
        //ICollection<FaceToRecognize> GetFacesToRecognize();

        public Task<FaceToRecognize> GetFaceToRecognize(int id);

        //FaceToRecognize GetFaceToRecognizeByDateTime(DateTime dateTime);

        //FaceToRecognize GetFaceToTrainByImageName(string imageFile);

        //bool FaceToRecognizeExists(int id);

        //void DetachTracking(FaceToRecognize faceToRecognize);
        public Task<FaceToRecognize> CreateFaceToRecognize(FaceToRecognize faceToRecognize);

        //bool UpdateFaceToRecognize(FaceToRecognize faceToRecognize);

        //bool DeleteFaceToRecognize(FaceToRecognize faceToRecognize);

        //bool Save();
    }
}
