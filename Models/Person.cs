using System.ComponentModel.DataAnnotations;

namespace FaceRecognitionWebAPI.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public int PairId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public ICollection<FaceToTrain> FacesToTrain { get; set; }

        public ICollection<FaceRecognitionStatus> FaceRecognitionStatuses { get; set; }
    }
}
