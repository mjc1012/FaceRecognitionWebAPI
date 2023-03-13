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

        public int RecognizeFace(FaceToRecognize face)
        {
            ITransformer mlModel = _mlContext.Model.Load(Path.Combine(_environment.ContentRootPath, "Face Recognition Model\\FaceRecognitionModel.zip"), out var _);
            Lazy<PredictionEngine<ModelInput, ModelOutput>> _PredictEngine = new(() => _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel), true);

            var path = Path.Combine(Path.Combine(_environment.ContentRootPath, "Faces To Recognize"), face.ImageFile);

            Bitmap image = new(path);
            image = SharpenImage(image);
            image = HistogramEqualizationColored(image);

            ModelInput faceData = new()
            {
                Image = image.ToMat().ToBytes(),
            };

            ModelOutput prediction = _PredictEngine.Value.Predict(faceData);

            return prediction.PredictedLabel;
        }

        public Bitmap SharpenImage(Bitmap image)
        {
            GaussianSharpen filter = new(4, 11);
            return filter.Apply(image);
        }

        public Bitmap HistogramEqualizationColored(Bitmap Image)
        {
            HistogramEqualization filter = new();
            filter.ApplyInPlace(Image);
            return Image;
        }

        public bool TrainModel()
        {
            IEnumerable<ImageData> images = LoadImagesFromDirectory(Path.Combine(_environment.ContentRootPath, "Augmented Faces"));

            IDataView imageData = _mlContext.Data.LoadFromEnumerable(images);

            IDataView shuffledData = _mlContext.Data.ShuffleRows(imageData);

            var preprocessingPipeline = _mlContext.Transforms.Conversion.MapValueToKey(
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
            return true;
        }

        public EstimatorChain<KeyToValueMappingTransformer> TrainingPipeline(MLContext mlContext, IDataView validationSet)
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
                    LearningRateScheduler = new Microsoft.ML.Trainers.LsrDecay(),
                    MetricsCallback = (metrics) => Console.WriteLine(metrics),
                })
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
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
