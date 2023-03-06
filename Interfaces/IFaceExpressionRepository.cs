using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceExpressionRepository
    {
        public Task<List<FaceExpression>> GetFaceExpressions();

        public Task<FaceExpression> GetFaceExpression(int id);

        //FaceExpression GetFaceExpressionByName(string name);


        //int GetFaceExpressionsCount();

        //bool FaceExpressionExists(int id);

        //bool CreateFaceExpression(FaceExpression faceExpression);

        //bool UpdateFaceExpression(FaceExpression faceExpression);

        //bool DeleteFaceExpression(FaceExpression faceExpression);

        //bool Save();
    }
}
