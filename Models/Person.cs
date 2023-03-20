using System.ComponentModel.DataAnnotations;

namespace FaceRecognitionWebAPI.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public string PairId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Password { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }


        public ICollection<FaceToTrain> FacesToTrain { get; set; }

        public ICollection<FaceRecognitionStatus> FaceRecognitionStatuses { get; set; }
    }
}
