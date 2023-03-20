using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Repository;
using FaceRecognitionWebAPI.Respository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp;

namespace FaceRecognitionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceToRecognizeController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public FaceToRecognizeController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> CreateFaceToRecognize([FromBody] FaceToRecognizeDto request)
        {
            ResponseDto<FaceToRecognizeDto> response;
            try
            {
                request.ImageFile = _uow.imageService.SaveImage(request.Base64String, -1);
                FaceToRecognize face = _mapper.Map<FaceToRecognize>(request);

                FaceToRecognize faceCreated = await _uow.faceToRecognizeRepository.CreateFaceToRecognize(face);

                if (faceCreated != null)
                {
                    response = new ResponseDto<FaceToRecognizeDto>() { Status = true, Message = "Face To Recognize Created", Value = _mapper.Map<FaceToRecognizeDto>(faceCreated) };
                }
                else
                {
                    response = new ResponseDto<FaceToRecognizeDto>() { Status = false, Message = "Face To Recognize Not Created" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceToRecognizeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}
