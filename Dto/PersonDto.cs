namespace FaceRecognitionWebAPI.Dto
{
    public class PersonDto
    {
        public int Id { get; set; }

        public int PairId { get; set; }
        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        public string LastName { get; set; }

    }
}
