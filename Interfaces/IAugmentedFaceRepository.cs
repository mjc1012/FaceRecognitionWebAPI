using FaceRecognitionWebAPI.Models;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IAugmentedFaceRepository
    {
        public Task CreateAugmentedFace(AugmentedFace augmentedFace);

        //ICollection<AugmentedFace> GetAugmentedFaces();

        //ICollection<AugmentedFace> GetAugmentedFacesByPersonId(int id);

        public Task<List<AugmentedFace>> GetAugmentedFaces(int faceToTrainId);

        //ICollection<AugmentedFace> GetAugmentedFacesByExpressionId(int id);

        //AugmentedFace GetAugmentedFaceById(int id);


        //bool AugmentedFaceExists(int id);

        //bool UpdateAugmentedFace(AugmentedFace augmentedFace);

        //bool DeleteAugmentedFace(AugmentedFace augmentedFace);

        //bool Save();
    }
}
