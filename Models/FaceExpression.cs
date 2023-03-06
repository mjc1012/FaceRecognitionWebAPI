namespace FaceRecognitionWebAPI.Models
{
    public class FaceExpression
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageFile { get; set; }

        public ICollection<FaceToTrain>? FacesToTrain { get; set; }
    }
}
