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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FaceExpression> GetFaceExpression(int id)
        {
            try
            {
                return await _context.FaceExpressions.Where(p => p.Id == id).Include(p => p.FacesToTrain).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
