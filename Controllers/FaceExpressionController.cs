using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Repository;
using FaceRecognitionWebAPI.Respository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceRecognitionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceExpressionController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public FaceExpressionController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFaceExpressions()
        {
            ResponseApi<List<FaceExpressionDto>> response;
            try
            {
                List<FaceExpressionDto> faceExpressions = _mapper.Map<List<FaceExpressionDto>>(await _uow.faceExpressionRepository.GetFaceExpressions());

                if (faceExpressions.Count > 0)
                {
                    response = new ResponseApi<List<FaceExpressionDto>>() { Status = true, Message = "Got All Face Expressions", Value = faceExpressions };
                }
                else
                {
                    response = new ResponseApi<List<FaceExpressionDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<FaceExpressionDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}
