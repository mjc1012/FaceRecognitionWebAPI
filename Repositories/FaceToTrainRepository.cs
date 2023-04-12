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
                return await _context.FacesToTrain.OrderByDescending(p => p.Id).Include(p => p.Person).Include(p => p.FaceExpression).Include(p => p.AugmentedFaces).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FaceToTrain>> GetFacesToTrain(int pairId)
        {
            try
            {
                return await _context.FacesToTrain.Where(p => p.Person.PairId == pairId).Include(p => p.Person).Include(p => p.FaceExpression).Include(p => p.AugmentedFaces).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FaceToTrain> GetFaceToTrain(int id)
        {
            try
            {
                return await _context.FacesToTrain.Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> FaceToTrainWithFaceExpressionExists(int pairId, int expressionId)
        {
           
            try
            {
                return await _context.FacesToTrain.AnyAsync(p => p.Person.PairId == pairId && p.FaceExpression.Id == expressionId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FaceExpression> GetMissingFaceExpressionOfPerson(int pairId, List<FaceExpression> faceExpressions)
        {
            try
            {
                foreach (FaceExpression faceExpression in faceExpressions)
                {
                    if (!await FaceToTrainWithFaceExpressionExists(pairId, faceExpression.Id))
                    {
                        return faceExpression;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
