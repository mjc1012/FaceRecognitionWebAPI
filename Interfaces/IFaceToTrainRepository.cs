using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceToTrainRepository
    {
        public Task<List<FaceToTrain>> GetFacesToTrain();

        public Task<List<FaceToTrain>> GetFacesToTrain(int pairId);
        public Task<bool> FaceToTrainWithFaceExpressionExists(int pairId, int expressionId);

        public Task<FaceExpression> GetMissingFaceExpressionOfPerson(int pairId, List<FaceExpression> faceExpressions);

        public Task<FaceToTrain> GetFaceToTrain(int id);

        public Task<FaceToTrain> CreateFaceToTrain(FaceToTrain faceToTrain);

        public Task<bool> DeleteFaceToTrain(FaceToTrain faceToTrain);

    }
}
