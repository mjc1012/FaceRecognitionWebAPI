using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceRecognitionWebAPI.Models
{
    public class FaceToRecognize
    {
        [Key]
        public int Id { get; set; }
        public string ImageFile { get; set; }     

        public DateTime LoggedTime { get; set; }

        public FaceRecognitionStatus FaceRecognitionStatus { get; set; }
    }
}
