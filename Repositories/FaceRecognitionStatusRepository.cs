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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public ICollection<FaceRecognitionStatus> GetFaceRecognitionStatuses()
        //{
        //    return _context.FaceRecognitionStatuses.OrderBy(p => p.Id).ToList();
        //}

        //public ICollection<FaceRecognitionStatus> GetFaceRecognitionStatusesByPersonId(int id)
        //{
        //    return _context.FaceRecognitionStatuses.Where(p => p.PredictedPerson.Id == id).ToList();
        //}

        //public FaceRecognitionStatus GetFaceRecognitionStatusById(int id)
        //{
        //    return _context.FaceRecognitionStatuses.Where(p => p.Id == id).FirstOrDefault();
        //}

        //public FaceRecognitionStatus GetFaceRecognitionStatusByFaceToRecognizeId(int id)
        //{
        //    return _context.FaceRecognitionStatuses.Where(p => p.FaceToRecognizeId == id).FirstOrDefault();
        //}

        //public bool FaceRecognitionStatusExists(int id)
        //{
        //    return _context.FaceRecognitionStatuses.Any(p => p.Id == id);
        //}

        //public bool UpdateFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus)
        //{
        //    _context.Update(faceRecognitionStatus);

        //    return Save();
        //}

        //public bool DeleteFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus)
        //{
        //    _context.Remove(faceRecognitionStatus);

        //    return Save();
        //}

        //public bool Save()
        //{
        //    var saved = _context.SaveChanges();
        //    return saved > 0;
        //}

    }
}
