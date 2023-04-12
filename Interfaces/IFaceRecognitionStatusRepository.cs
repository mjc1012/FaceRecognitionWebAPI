using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceRecognitionStatusRepository
    {
        public Task<List<FaceRecognitionStatus>> GetFaceRecognitionStatuses();
        public Task<FaceRecognitionStatus> CreateFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus);
    }
}
