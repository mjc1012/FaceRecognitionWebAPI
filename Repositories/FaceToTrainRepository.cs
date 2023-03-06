using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FaceRecognitionWebAPI.Repository
{
    public class FaceToTrainRepository : IFaceToTrainRepository
    {
        private readonly DataContext _context;
        public FaceToTrainRepository(DataContext context)
        {
            _context = context;

        }

        public async Task<List<FaceToTrain>> GetFacesToTrain()
        {
            try
            {
                return await _context.FacesToTrain.OrderBy(p => p.Id).Include(p => p.Person).Include(p => p.FaceExpression).Include(p => p.AugmentedFaces).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<FaceToTrain>> GetFacesToTrain(int personId)
        {
            try
            {
                return await _context.FacesToTrain.Where(p => p.Person.Id == personId).Include(p => p.Person).Include(p => p.FaceExpression).Include(p => p.AugmentedFaces).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FaceToTrain> GetFaceToTrain(int id)
        {
            try
            {
                return await _context.FacesToTrain.Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> FaceToTrainWithFaceExpressionExists(int personId, int expressionId)
        {
           
            try
            {
                return await _context.FacesToTrain.AnyAsync(p => p.Person.Id == personId && p.FaceExpression.Id == expressionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<FaceExpression> GetMissingFaceExpressionOfPerson(int id, List<FaceExpression> faceExpressions)
        {
            try
            {
                foreach (var faceExpression in faceExpressions)
                {
                    if (!(await FaceToTrainWithFaceExpressionExists(id, faceExpression.Id)))
                    {
                        return faceExpression;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<FaceToTrain> CreateFaceToTrain(FaceToTrain faceToTrain)
        {
            try
            {
                _context.FacesToTrain.Add(faceToTrain);
                await _context.SaveChangesAsync();
                return faceToTrain;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<bool> DeleteFaceToTrain(FaceToTrain faceToTrain)
        {
            try
            {
                _context.FacesToTrain.Remove(faceToTrain);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool Save()
        //{
        //    var saved = _context.SaveChanges();
        //    return saved > 0;
        //}

        //public bool UpdateFaceToTrain(FaceToTrain faceToTrain)
        //{
        //    _context.Update(faceToTrain);

        //    return Save();
        //}

        //public bool FaceToTrainExists(int id)
        //{
        //    return _context.FacesToTrain.Any(p => p.Id == id);
        //}

        //public FaceToTrain GetFaceToTrainByImageName(string imageFile)
        //{
        //    return _context.FacesToTrain.Where(p => p.ImageFile.Trim() == imageFile.Trim()).FirstOrDefault();
        //}


        

        //public ICollection<FaceToTrain> GetFacesToTrainByExpressionId(int id)
        //{
        //    return _context.FacesToTrain.Where(p => p.FaceExpression.Id == id).ToList();

        //}

        //public int GetFacesToTrainCountByPersonId(int id)
        //{
        //    return _context.FacesToTrain.Where(p => p.Person.Id == id).Count();
        //}
    }
}
