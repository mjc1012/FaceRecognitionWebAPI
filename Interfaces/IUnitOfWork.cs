namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IUnitOfWork
    {
        IAugmentedFaceRepository augmentedFaceRepository { get; }
        IFaceExpressionRepository faceExpressionRepository { get; }
        IFaceRecognitionService faceRecognitionService { get; }
        IFaceRecognitionStatusRepository faceRecognitionStatusRepository { get; }
        IFaceToRecognizeRepository faceToRecognizeRepository { get; }
        IFaceToTrainRepository faceToTrainRepository { get; }
        IImageAugmentationService imageAugmentationService { get; }
        IImageService imageService { get; }
        IPersonRepository personRepository { get; }
    }
}
