using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceRecognitionWebAPI.Models
{
    public class AugmentedFace
    {
        public int Id { get; set; }

        public string? ImageFile { get; set; }

        public int FaceToTrainId { get; set; }   

        public FaceToTrain FaceToTrain { get; set; }
    }
}
