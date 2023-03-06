namespace FaceRecognitionWebAPI.Dto
{
    public class PersonDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string ValidIdNumber { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
