using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IAugmentedFaceRepository
    {
        public Task CreateAugmentedFace(AugmentedFace augmentedFace);

        public Task<List<AugmentedFace>> GetAugmentedFaces(int faceToTrainId);
    }
}
