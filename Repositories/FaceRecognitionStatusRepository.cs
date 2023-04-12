using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace FaceRecognitionWebAPI.Repository
{
    public class FaceRecognitionStatusRepository : IFaceRecognitionStatusRepository
    {
        private readonly DataContext _context;
        public FaceRecognitionStatusRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<FaceRecognitionStatus>> GetFaceRecognitionStatuses()
        {
            try
            {
                return await _context.FaceRecognitionStatuses.OrderBy(p => p.FaceToRecognize.LoggedTime).ThenBy(p => p.PredictedPerson.LastName).ThenBy(p => p.PredictedPerson.MiddleName).Include(p => p.PredictedPerson.LastName).Include(p => p.FaceToRecognize).Include(p => p.PredictedPerson).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FaceRecognitionStatus> CreateFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus)
        {
            try
            {
                _context.FaceRecognitionStatuses.Add(faceRecognitionStatus);
                await _context.SaveChangesAsync();
                return faceRecognitionStatus;
            }
            catch (Exception )
            {
                throw ;
            }
        }
    }
}
