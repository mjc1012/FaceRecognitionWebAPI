using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Models;
using System.Globalization;

namespace FaceRecognitionWebAPI.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Person, PersonDto>();
            CreateMap<PersonDto, Person>()
                .ForMember(
                destiny => destiny.FacesToTrain,
                opt => opt.Ignore()
                )
                .ForMember(
                destiny => destiny.FaceRecognitionStatuses,
                opt => opt.Ignore()
                );

            CreateMap<FaceToTrain, FaceToTrainDto>()
                .ForMember(
                destiny => destiny.Base64String,
                opt => opt.Ignore()
                );
            CreateMap<FaceToTrainDto, FaceToTrain>()
                .ForMember(
                destiny => destiny.Person,
                opt => opt.Ignore()
                )
                .ForMember(
                destiny => destiny.FaceExpression,
                opt => opt.Ignore()
                );

            CreateMap<FaceToRecognize, FaceToRecognizeDto>()
                .ForMember(
                destiny => destiny.LoggedTime,
                opt => opt.MapFrom(origin => origin.LoggedTime.ToString("yyyy-MM-dd HH:mm:ss"))
                )
                .ForMember(
                destiny => destiny.Base64String,
                opt => opt.Ignore()
                );
            CreateMap<FaceToRecognizeDto, FaceToRecognize>()
               .ForMember(
               destiny => destiny.LoggedTime,
               opt => opt.MapFrom(origin => DateTime.ParseExact(origin.LoggedTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
               )
               .ForMember(
               destiny => destiny.FaceRecognitionStatus,
               opt => opt.Ignore()
               );

            CreateMap<FaceExpression, FaceExpressionDto>();
            CreateMap<FaceExpressionDto, FaceExpression>()
                .ForMember(
                destiny => destiny.FacesToTrain,
                opt => opt.Ignore()
                );

            CreateMap<AugmentedFace, AugmentedFaceDto>();
            CreateMap<AugmentedFaceDto, AugmentedFace>()
                .ForMember(
                destiny => destiny.FaceToTrain,
                opt => opt.Ignore()
                ); ;

            CreateMap<FaceRecognitionStatus, FaceRecognitionStatusDto>();
            CreateMap<FaceRecognitionStatusDto, FaceRecognitionStatus>()
                .ForMember(
                destiny => destiny.FaceToRecognize,
                opt => opt.Ignore()
                )
                .ForMember(
                destiny => destiny.PredictedPerson,
                opt => opt.Ignore()
                );
        }
    }
}
