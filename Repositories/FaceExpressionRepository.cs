using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceRecognitionWebAPI.Repository
{
    public class FaceExpressionRepository : IFaceExpressionRepository
    {
        private readonly DataContext _context;
        public FaceExpressionRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<List<FaceExpression>> GetFaceExpressions()
        {
            try
            {
                return await _context.FaceExpressions.OrderBy(p => p.Id).Include(p => p.FacesToTrain).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FaceExpression> GetFaceExpression(int id)
        {
            return await _context.FaceExpressions.Where(p => p.Id == id).Include(p => p.FacesToTrain).FirstOrDefaultAsync();
        }
        //public int GetFaceExpressionsCount()
        //{
        //    return _context.FaceExpressions.OrderBy(p => p.Id).Count();
        //}


        //public FaceExpression GetFaceExpressionByName(string name)
        //{
        //    return _context.FaceExpressions.Where(p => p.Name.Trim() == name.Trim()).FirstOrDefault();
        //}

        //public bool FaceExpressionExists(int id)
        //{
        //    return _context.FaceExpressions.Any(p => p.Id == id);
        //}

        //public bool CreateFaceExpression(FaceExpression faceExpression)
        //{
        //    _context.FaceExpressions.Add(faceExpression);

        //    return Save();
        //}

        //public bool UpdateFaceExpression(FaceExpression faceExpression)
        //{
        //    _context.Update(faceExpression);

        //    return Save();
        //}

        //public bool DeleteFaceExpression(FaceExpression faceExpression)
        //{
        //    _context.Remove(faceExpression);

        //    return Save();
        //}

        //public bool Save()
        //{
        //    var saved = _context.SaveChanges();
        //    return saved > 0;
        //}
    }
}
