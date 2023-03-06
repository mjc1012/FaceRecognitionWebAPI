using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceToTrainRepository
    {
        public Task<List<FaceToTrain>> GetFacesToTrain();

        public Task<List<FaceToTrain>> GetFacesToTrain(int personId);
        public Task<bool> FaceToTrainWithFaceExpressionExists(int personId, int expressionId);

        public Task<FaceExpression> GetMissingFaceExpressionOfPerson(int id, List<FaceExpression> faceExpressions);

        public Task<FaceToTrain> GetFaceToTrain(int id);

        public Task<FaceToTrain> CreateFaceToTrain(FaceToTrain faceToTrain);

        public Task<bool> DeleteFaceToTrain(FaceToTrain faceToTrain);

        //bool Save();

        //ICollection<FaceToTrain> GetFacesToTrainByExpressionId(int id);


        //int GetFacesToTrainCountByPersonId(int id);

        //FaceToTrain GetFaceToTrainByImageName(string imageFile);

        

        //bool FaceToTrainExists(int id);

        //bool UpdateFaceToTrain(FaceToTrain faceToTrain);
    }
}
