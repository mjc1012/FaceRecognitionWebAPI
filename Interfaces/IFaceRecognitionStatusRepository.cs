using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceRecognitionStatusRepository
    {
        public Task<FaceRecognitionStatus> CreateFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus);
        //ICollection<FaceRecognitionStatus> GetFaceRecognitionStatuses();

        //ICollection<FaceRecognitionStatus> GetFaceRecognitionStatusesByPersonId(int id);

        //FaceRecognitionStatus GetFaceRecognitionStatusById(int id);

        //FaceRecognitionStatus GetFaceRecognitionStatusByFaceToRecognizeId(int id);

        //bool FaceRecognitionStatusExists(int id);


        //bool UpdateFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus);

        //bool DeleteFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus);
        //bool Save();
    }
}
