using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Repository;
using FaceRecognitionWebAPI.Respository;
using Microsoft.AspNetCore.Mvc;

namespace FaceRecognitionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceExpressionController : Controller
    {
        private readonly IFaceExpressionRepository _faceExpressionRepository;
        private readonly IMapper _mapper;
        public FaceExpressionController(IFaceExpressionRepository faceExpressionRepository, IMapper mapper)
        {
            _faceExpressionRepository = faceExpressionRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaceExpressions()
        {
            ResponseApi<List<FaceExpressionDto>> response;
            try
            {
                List<FaceExpressionDto> faceExpressions = _mapper.Map<List<FaceExpressionDto>>(await _faceExpressionRepository.GetFaceExpressions());

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

        //[HttpGet("expression-count")]
        //[ProducesResponseType(200, Type = typeof(int))]
        //[ProducesResponseType(400)]
        //public IActionResult GetFacesToTrainCountByPersonId()
        //{
        //    var count = _faceExpressionRepository.GetFaceExpressionsCount();

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(count);
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(200, Type = typeof(FaceExpression))]
        //[ProducesResponseType(400)]
        //public IActionResult GetFaceExpressionById(int id)
        //{
        //    if (!_faceExpressionRepository.FaceExpressionExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var faceExpression = _mapper.Map<FaceExpressionDto>(_faceExpressionRepository.GetFaceExpressionById(id));

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(faceExpression);
        //}

        //[HttpPost]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(400)]
        //public IActionResult CreateFaceExpression([FromBody] FaceExpressionDto faceExpressionCreate)
        //{
        //    if (faceExpressionCreate == null)
        //        return BadRequest(ModelState);

        //    var faceExpression = _faceExpressionRepository.GetFaceExpressionByName(faceExpressionCreate.Name);

        //    if (faceExpression != null)
        //    {
        //        ModelState.AddModelError("", "Face expression already exists");
        //        return StatusCode(422, ModelState);
        //    }

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var faceExpressionMap = _mapper.Map<FaceExpression>(faceExpressionCreate);

        //    if (!_faceExpressionRepository.CreateFaceExpression(faceExpressionMap))
        //    {
        //        ModelState.AddModelError("", "Something went wrong while saving");
        //        return StatusCode(500, ModelState);
        //    }

        //    return Ok("Successfully created");
        //}

        //[HttpPut("{id}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public IActionResult UpdateFaceExpression(int id, [FromBody] FaceExpressionDto updateFaceExpression)
        //{
        //    if (updateFaceExpression == null)
        //        return BadRequest(ModelState);

        //    if (id != updateFaceExpression.Id)
        //        return BadRequest(ModelState);

        //    if (!_faceExpressionRepository.FaceExpressionExists(id))
        //        return NotFound();

        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    var faceExpressionMap = _mapper.Map<FaceExpression>(updateFaceExpression);

        //    if (!_faceExpressionRepository.UpdateFaceExpression(faceExpressionMap))
        //    {
        //        ModelState.AddModelError("", "Something went wrong updating face expression");
        //        return StatusCode(500, ModelState);
        //    }

        //    return Ok("Successfully updated");
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public IActionResult DeleteFaceExpression(int id)
        //{
        //    if (!_faceExpressionRepository.FaceExpressionExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var faceExpresssion = _faceExpressionRepository.GetFaceExpressionById(id);

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    if (!_faceExpressionRepository.DeleteFaceExpression(faceExpresssion))
        //    {
        //        ModelState.AddModelError("", "Something went wrong deleting face expression");
        //    }

        //    return Ok("Successfully deleted");
        //}
    }
}
