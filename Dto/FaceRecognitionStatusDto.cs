namespace FaceRecognitionWebAPI.Dto
{
    public class FaceRecognitionStatusDto
    {
        public int Id { get; set; }
        public bool IsRecognized { get; set; }

        public int FaceToRecognizeId { get; set; }

        public int PredictedPersonId { get; set; }
    }
}
