using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Repository;
using FaceRecognitionWebAPI.Respository;

namespace FaceRecognitionWebAPI.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;
        public UnitOfWork(DataContext context, IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        public IAugmentedFaceRepository augmentedFaceRepository => new AugmentedFaceRepository(_context);
        public IAuthenticationService authenticationRepository => new AuthenticationService(_context);
        public IFaceExpressionRepository faceExpressionRepository => new FaceExpressionRepository(_context);
        public IFaceRecognitionStatusRepository faceRecognitionStatusRepository => new FaceRecognitionStatusRepository(_context);
        public IFaceToRecognizeRepository faceToRecognizeRepository => new FaceToRecognizeRepository(_context);
        public IPersonRepository personRepository => new PersonRepository(_context);
        public IFaceRecognitionService faceRecognitionService => new FaceRecognitionService(_environment);
        public IFaceToTrainRepository faceToTrainRepository => new FaceToTrainRepository(_context);

        public IImageAugmentationService imageAugmentationService => new ImageAugmentationService(augmentedFaceRepository, imageService, _environment);

        public IImageService imageService => new ImageService(_environment, faceRecognitionService);
    }
}
