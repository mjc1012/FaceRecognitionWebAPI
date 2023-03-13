using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FaceRecognitionWebAPI.Repository
{
    public class FaceToRecognizeRepository : IFaceToRecognizeRepository
    {
        private readonly DataContext _context;
        public FaceToRecognizeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<FaceToRecognize> GetFaceToRecognize(int id)
        {
            try
            {
                return await _context.FacesToRecognize.Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FaceToRecognize> CreateFaceToRecognize(FaceToRecognize faceToRecognize)
        {
            try
            {
                _context.FacesToRecognize.Add(faceToRecognize);
                await _context.SaveChangesAsync();
                return faceToRecognize;
            }
            catch (Exception )
            {
                throw;
            }
        }

    }
}
