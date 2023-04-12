namespace FaceRecognitionWebAPI.Dto
{
    public class FaceToTrainDto
    {
        public int Id { get; set; }

        public string ImageFile { get; set; }

        public string Base64String { get; set; }

        public int PairId { get; set; }

        public int FaceExpressionId { get; set; }
    }
}
