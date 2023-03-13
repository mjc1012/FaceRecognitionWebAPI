using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
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
