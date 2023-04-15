using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;
using System.Drawing;
using System.IO.Compression;
using Microsoft.ML;
using Tensorflow;
using FaceRecognitionWebAPI.Face_Recognition_Model;
using static Microsoft.ML.DataOperationsCatalog;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using Microsoft.ML.Vision;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.CompilerServices;
using AForge.Imaging.Filters;

namespace FaceRecognitionWebAPI.Services
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private readonly MLContext _mlContext;
        private readonly IWebHostEnvironment _environment;

        public FaceRecognitionService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _mlContext = new();
        }

        public Mat DetectFace(Bitmap img)
        {
            try
            {
                Net faceNet = CvDnn.ReadNetFromCaffe(Path.Combine(_environment.ContentRootPath, "Face Detection Model\\deploy.prototxt.txt"), Path.Combine(_environment.ContentRootPath, "Face Detection Model\\res10_300x300_ssd_iter_140000_fp16.caffemodel"));

                Mat newImage = img.ToMat();
                Cv2.CvtColor(newImage, newImage, ColorConversionCodes.RGBA2RGB);
                int frameHeight = newImage.Rows;
                int frameWidth = newImage.Cols;

                Mat blob = CvDnn.BlobFromImage(newImage, 1.0, new Size(224, 224),
                    new Scalar(104, 117, 123), false, false);

                faceNet.SetInput(blob, "data");

                Mat detection = faceNet.Forward("detection_out");
                Mat detectionMat = new(detection.Size(2), detection.Size(3), MatType.CV_32F,
                    detection.Ptr(0));

                float confidence = detectionMat.At<float>(0, 2);
                int x1 = (int)(detectionMat.At<float>(0, 3) * frameWidth);
                int y1 = (int)(detectionMat.At<float>(0, 4) * frameHeight);
                int x2 = (int)(detectionMat.At<float>(0, 5) * frameWidth);
                int y2 = (int)(detectionMat.At<float>(0, 6) * frameHeight);

                Rect roi = new(x1, y1, x2 - x1, y2 - y1);
                return newImage.Clone(roi);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int RecognizeFace(FaceToRecognize face)
        {
            try
            {
                ITransformer mlModel = _mlContext.Model.Load(Path.Combine(_environment.ContentRootPath, "Face Recognition Model\\FaceRecognitionModel.zip"), out var _);
                Lazy<PredictionEngine<ModelInput, ModelOutput>> _PredictEngine = new(() => _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel), true);

                string path = Path.Combine(Path.Combine(_environment.ContentRootPath, "Faces To Recognize"), face.ImageFile);

                Bitmap image = new(path);
                image = SharpenImage(image);
                image = HistogramEqualizationColored(image);

                ModelInput faceData = new()
                {
                    Image = image.ToMat().ToBytes(),
                };

                ModelOutput prediction = _PredictEngine.Value.Predict(faceData);

                if(prediction.Score.Max() > 0.9)
                {
                    return prediction.PredictedLabel;
                }
                return -1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Bitmap SharpenImage(Bitmap image)
        {
            try
            {
                GaussianSharpen filter = new(4, 11);
                return filter.Apply(image);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Bitmap HistogramEqualizationColored(Bitmap Image)
        {
            try
            {
                HistogramEqualization filter = new();
                filter.ApplyInPlace(Image);
                return Image;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void TrainModel()
        {
            try
            {
                IEnumerable<ImageData> images = LoadImagesFromDirectory(Path.Combine(_environment.ContentRootPath, "Augmented Faces"));

                IDataView imageData = _mlContext.Data.LoadFromEnumerable(images);

                IDataView shuffledData = _mlContext.Data.ShuffleRows(imageData);

                EstimatorChain<ImageLoadingTransformer> preprocessingPipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                                                                            inputColumnName: "Label",
                                                                            outputColumnName: "LabelAsKey")
                                            .Append(_mlContext.Transforms.LoadRawImageBytes(
                                                                            outputColumnName: "Image",
                                                                            imageFolder: Path.Combine(_environment.ContentRootPath, "Augmented Faces"),
                                                                            inputColumnName: "ImagePath"));

                IDataView preProcessedData = preprocessingPipeline.Fit(shuffledData).Transform(shuffledData);

                TrainTestData trainSplit = _mlContext.Data.TrainTestSplit(data: preProcessedData, testFraction: 0.1);

                IDataView trainSet = trainSplit.TrainSet;
                IDataView validationSet = trainSplit.TestSet;

                ITransformer trainedModel = TrainingPipeline(_mlContext, validationSet).Fit(trainSet);

                _mlContext.Model.Save(trainedModel, trainSet.Schema, Path.Combine(_environment.ContentRootPath, "Face Recognition Model\\FaceRecognitionModel.zip"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EstimatorChain<KeyToValueMappingTransformer> TrainingPipeline(MLContext mlContext, IDataView validationSet)
        {
            try
            {
                return mlContext.MulticlassClassification.Trainers.ImageClassification(
                new ImageClassificationTrainer.Options()
                {
                    FeatureColumnName = "Image",
                    LabelColumnName = "LabelAsKey",
                    ValidationSet = validationSet,
                    Arch = ImageClassificationTrainer.Architecture.MobilenetV2,
                    Epoch = 1000000,
                    BatchSize = 32,
                    EarlyStoppingCriteria = new ImageClassificationTrainer.EarlyStopping(),
                    LearningRateScheduler = new Microsoft.ML.Trainers.LsrDecay(),
                    MetricsCallback = (metrics) => Console.WriteLine(metrics),
                })
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ImageData> LoadImagesFromDirectory(string folder)
        {
            var files = Directory.GetFiles(folder, "*",
                searchOption: SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png") && (Path.GetExtension(file) != ".jpeg"))
                    continue;

                var label = Int32.Parse(Directory.GetParent(file).Name);

                yield return new ImageData()
                {
                    ImagePath = file,
                    Label = label
                };
            }
        }
    }
}
