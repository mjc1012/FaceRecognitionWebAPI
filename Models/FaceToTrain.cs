using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceRecognitionWebAPI.Models
{
    public class FaceToTrain
    {
        [Key]
        public int Id { get; set; }
        public string ImageFile { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }

        public int FaceExpressionId { get; set; }

        public FaceExpression FaceExpression { get; set; }

        public ICollection<AugmentedFace> AugmentedFaces { get; set; }
    }
}
