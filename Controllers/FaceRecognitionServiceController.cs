using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp.Face;
using Tensorflow;

namespace FaceRecognitionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceRecognitionServiceController : Controller
    {
        private readonly IFaceToRecognizeRepository _faceToRecognizeRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IFaceRecognitionService _faceRecognitionService;
        private readonly IMapper _mapper;
        public FaceRecognitionServiceController(IFaceRecognitionService faceRecognitionService, IFaceToRecognizeRepository faceToRecognizeRepository,
            IPersonRepository personRepository, IMapper mapper)
        {
            _faceToRecognizeRepository = faceToRecognizeRepository;
            _personRepository = personRepository;
            _faceRecognitionService = faceRecognitionService; ;
            _mapper = mapper;
        }

        [HttpGet("recognize-face/{id}")]
        public async Task<IActionResult> RecognizeFace(int id)
        {
            ResponseApi<PersonDto> response;

            var face = await _faceToRecognizeRepository.GetFaceToRecognize(id);
            int predictedPersonId = _faceRecognitionService.RecognizeFace(face);
            var predictedPerson = await _personRepository.GetPerson(predictedPersonId);
            PersonDto person = _mapper.Map<PersonDto>(predictedPerson);

                if (person != null)
                {
                    response = new ResponseApi<PersonDto>() { Status = true, Message = "Got Predicted Person", Value = person };
                }
                else
                {
                    response = new ResponseApi<PersonDto>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
           
        }

        [Authorize]
        [HttpGet("train-model")]
        public IActionResult TrainModel()
        {

            ResponseApi<bool> response;

            if (_faceRecognitionService.TrainModel())
            {
                response = new ResponseApi<bool>() { Status = true, Message = "Model Successfully Trained" };
            }
            else
            {
                response = new ResponseApi<bool>() { Status = false, Message = "Training Model was Unsuccessful" };
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
