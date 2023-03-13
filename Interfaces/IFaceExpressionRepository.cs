using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceExpressionRepository
    {
        public Task<List<FaceExpression>> GetFaceExpressions();

        public Task<FaceExpression> GetFaceExpression(int id);
    }
}
