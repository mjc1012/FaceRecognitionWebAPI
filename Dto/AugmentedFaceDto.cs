using System.ComponentModel.DataAnnotations.Schema;

namespace FaceRecognitionWebAPI.Dto
{
    public class AugmentedFaceDto
    {
        public int Id { get; set; }
        public string? ImageFile { get; set; }
        public int FaceToTrainId { get; set; }
    }
}
