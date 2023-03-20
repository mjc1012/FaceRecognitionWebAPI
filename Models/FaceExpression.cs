using System.ComponentModel.DataAnnotations;

namespace FaceRecognitionWebAPI.Models
{
    public class FaceExpression
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageFile { get; set; }

        public ICollection<FaceToTrain> FacesToTrain { get; set; }
    }
}
