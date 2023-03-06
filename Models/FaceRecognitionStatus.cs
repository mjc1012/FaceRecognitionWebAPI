namespace FaceRecognitionWebAPI.Models
{
    public class FaceRecognitionStatus
    {
        public int Id { get; set; }
        public bool IsRecognized { get; set; }
        public int FaceToRecognizeId { get; set; }
        public FaceToRecognize FaceToRecognize { get; set; }

        public int PredictedPersonId { get; set; }

        public Person PredictedPerson { get; set; }
    }
}
