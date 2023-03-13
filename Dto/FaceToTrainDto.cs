namespace FaceRecognitionWebAPI.Dto
{
    public class FaceToTrainDto
    {
        public int Id { get; set; }

        public string ImageFile { get; set; }

        public string Base64String { get; set; }

        public int PersonId { get; set; }

        public int FaceExpressionId { get; set; }
    }
}
