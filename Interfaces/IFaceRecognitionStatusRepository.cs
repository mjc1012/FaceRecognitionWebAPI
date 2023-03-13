﻿using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IFaceRecognitionStatusRepository
    {
        public Task<FaceRecognitionStatus> CreateFaceRecognitionStatus(FaceRecognitionStatus faceRecognitionStatus);
    }
}
