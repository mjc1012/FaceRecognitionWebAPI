using AutoMapper;
using FaceRecognitionWebAPI.Dto;
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
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public FaceRecognitionServiceController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet("recognize-face/{id}")]
        public async Task<IActionResult> RecognizeFace(int id)
        {
            ResponseDto<PersonDto> response;
            try
            {
                var face = await _uow.faceToRecognizeRepository.GetFaceToRecognize(id);
                int predictedPersonId = _uow.faceRecognitionService.RecognizeFace(face);
                if (predictedPersonId == -1)
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "No data" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                var predictedPerson = await _uow.personRepository.GetPersonById(predictedPersonId);
                PersonDto person = _mapper.Map<PersonDto>(predictedPerson);

                if (person != null)
                {
                    response = new ResponseDto<PersonDto>() { Status = true, Message = "Got Predicted Person", Value = person };
                }
                else
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }

        [HttpGet("train-model")]
        public IActionResult TrainModel()
        {
            ResponseDto<bool> response;
            try
            {
                _uow.faceRecognitionService.TrainModel();
                response = new ResponseDto<bool>() { Status = true, Message = "Model Successfully Trained" };
                
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch(Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }
    }
}
