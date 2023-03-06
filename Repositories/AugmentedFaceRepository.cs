using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FaceRecognitionWebAPI.Repository
{
    public class AugmentedFaceRepository : IAugmentedFaceRepository
    {
        private readonly DataContext _context;
        public AugmentedFaceRepository(DataContext context)
        {
            _context = context;
        }

        public async Task CreateAugmentedFace(AugmentedFace augmentedFace)
        {
            try
            {
                _context.AugmentedFaces.Add(augmentedFace);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public ICollection<AugmentedFace> GetAugmentedFaces()
        //{
        //    return _context.AugmentedFaces.OrderBy(p => p.Id).ToList();
        //}

        //public ICollection<AugmentedFace> GetAugmentedFacesByPersonId(int id)
        //{
        //    return _context.AugmentedFaces.Where(p => p.FaceToTrain.Person.Id == id).ToList();
        //}

        public async Task<List<AugmentedFace>> GetAugmentedFaces(int faceToTrainId)
        {
            try
            {
                return await _context.AugmentedFaces.Where(p => p.FaceToTrain.Id == faceToTrainId).Include(p => p.FaceToTrain).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public ICollection<AugmentedFace> GetAugmentedFacesByExpressionId(int id)
        //{
        //    return _context.AugmentedFaces.Where(p => p.FaceToTrain.FaceExpression.Id == id).ToList();
        //}

        //public AugmentedFace GetAugmentedFaceById(int id)
        //{
        //    return _context.AugmentedFaces.Where(p => p.Id == id).FirstOrDefault();
        //}

        //public bool AugmentedFaceExists(int id)
        //{
        //    return _context.AugmentedFaces.Any(p => p.Id == id);
        //}



        //public bool UpdateAugmentedFace(AugmentedFace augmentedFace)
        //{
        //    _context.Update(augmentedFace);

        //    return Save();
        //}

        //public bool DeleteAugmentedFace(AugmentedFace augmentedFace)
        //{
        //    _context.Remove(augmentedFace);

        //    return Save();
        //}

        //public bool Save()
        //{
        //    var saved = _context.SaveChanges();
        //    return saved > 0;
        //}
    }
}
