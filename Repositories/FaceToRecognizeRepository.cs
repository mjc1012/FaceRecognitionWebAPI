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

        //public ICollection<FaceToRecognize> GetFacesToRecognize()
        //{
        //    return _context.FacesToRecognize.OrderBy(p => p.Id).ToList();
        //}

        public async Task<FaceToRecognize> GetFaceToRecognize(int id)
        {
            try
            {
                return await _context.FacesToRecognize.Where(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public FaceToRecognize GetFaceToRecognizeByDateTime(DateTime dateTime)
        //{
        //    return _context.FacesToRecognize.Where(p => p.LoggedTime == dateTime).FirstOrDefault();
        //}

        //public FaceToRecognize GetFaceToTrainByImageName(string imageFile)
        //{
        //    return _context.FacesToRecognize.Where(p => p.ImageFile.Trim() == imageFile.Trim()).FirstOrDefault();
        //}


        //public bool FaceToRecognizeExists(int id)
        //{
        //    return _context.FacesToRecognize.Any(p => p.Id == id);
        //}

        //public void DetachTracking(FaceToRecognize faceToRecognize)
        //{

        //    _context.Entry(faceToRecognize).State = EntityState.Detached;
        //}

        public async Task<FaceToRecognize> CreateFaceToRecognize(FaceToRecognize faceToRecognize)
        {
            try
            {
                _context.FacesToRecognize.Add(faceToRecognize);
                await _context.SaveChangesAsync();
                return faceToRecognize;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool UpdateFaceToRecognize(FaceToRecognize faceToRecognize)
        //{
        //    _context.Update(faceToRecognize);

        //    return Save();
        //}

        //public bool DeleteFaceToRecognize(FaceToRecognize faceToRecognize)
        //{
        //    _context.Remove(faceToRecognize);

        //    return Save();
        //}

        //public bool Save()
        //{
        //    var saved = _context.SaveChanges();
        //    return saved > 0;
        //}
    }
}
