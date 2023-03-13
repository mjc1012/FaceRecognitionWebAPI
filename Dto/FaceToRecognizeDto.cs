namespace FaceRecognitionWebAPI.Dto
{
    public class FaceToRecognizeDto
    {
        public int Id { get; set; }
        public string ImageFile { get; set; }

        public string Base64String { get; set; }

        public string LoggedTime { get; set; }
    }
}
