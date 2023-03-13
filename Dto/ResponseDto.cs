namespace FaceRecognitionWebAPI.Dto
{
    public class ResponseDto<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }
    }
}
