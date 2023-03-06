namespace FaceRecognitionWebAPI.Face_Recognition_Model
{
    public class ModelInput
    {

        public byte[] Image { get; set; }

        public UInt32 LabelAsKey { get; set; }

        public string ImagePath { get; set; }

        public int Label { get; set; }
    }
}
