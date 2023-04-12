using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Repository;
using FaceRecognitionWebAPI.Respository;
using FaceRecognitionWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FaceRecognitionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceRecognitionStatusController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public FaceRecognitionStatusController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaceRecognitionStatuses()
        {
            ResponseDto<List<FaceRecognitionStatusDto>> response;
            try
            {
                List<FaceRecognitionStatusDto> faceRecognitionStatuses = _mapper.Map<List<FaceRecognitionStatusDto>>(await _uow.faceRecognitionStatusRepository.GetFaceRecognitionStatuses());

                if (faceRecognitionStatuses.Count > 0)
                {
                    response = new ResponseDto<List<FaceRecognitionStatusDto>>() { Status = true, Message = "Got All Face Recognition Statuses", Value = faceRecognitionStatuses };
                }
                else
                {
                    response = new ResponseDto<List<FaceRecognitionStatusDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<FaceRecognitionStatusDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaceRecognitionStatus([FromBody] FaceRecognitionStatusDto request)
        {
            ResponseDto<FaceRecognitionStatusDto> response;
            try
            {
                var status = _mapper.Map<FaceRecognitionStatus>(request);
                status.PredictedPerson = await _uow.personRepository.GetPersonById(request.PredictedPersonId);
                status.FaceToRecognize = await _uow.faceToRecognizeRepository.GetFaceToRecognize(request.FaceToRecognizeId);

                FaceRecognitionStatus statusCreated = await _uow.faceRecognitionStatusRepository.CreateFaceRecognitionStatus(status);


                if (statusCreated != null)
                {
                    response = new ResponseDto<FaceRecognitionStatusDto>() { Status = true, Message = "Face Recognition Status Created", Value = _mapper.Map<FaceRecognitionStatusDto>(statusCreated) };
                }
                else
                {
                    response = new ResponseDto<FaceRecognitionStatusDto>() { Status = false, Message = "Face Recognition Status Not Created" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceRecognitionStatusDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


    }
}
