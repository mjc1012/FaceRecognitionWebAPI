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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<AugmentedFace>> GetAugmentedFaces(int faceToTrainId)
        {
            try
            {
                return await _context.AugmentedFaces.Where(p => p.FaceToTrain.Id == faceToTrainId).Include(p => p.FaceToTrain).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
