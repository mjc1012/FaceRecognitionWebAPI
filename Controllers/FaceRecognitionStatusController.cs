using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
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
        private readonly IPersonRepository _personRepository;
        private readonly IFaceRecognitionStatusRepository _faceRecognitionStatusRepository;
        private readonly IFaceToRecognizeRepository _faceToRecognizeRepository;
        private readonly IMapper _mapper;
        public FaceRecognitionStatusController(IPersonRepository personRepository, IFaceRecognitionStatusRepository faceRecognitionStatusRepository, IFaceToRecognizeRepository faceToRecognizeRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _faceRecognitionStatusRepository = faceRecognitionStatusRepository;
            _faceToRecognizeRepository = faceToRecognizeRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> CreateFaceRecognitionStatus([FromBody] FaceRecognitionStatusDto request)
        {
            ResponseApi<FaceRecognitionStatusDto> response;
            try
            {
                var status = _mapper.Map<FaceRecognitionStatus>(request);
                status.PredictedPerson = await _personRepository.GetPerson(request.PredictedPersonId);
                status.FaceToRecognize = await _faceToRecognizeRepository.GetFaceToRecognize(request.FaceToRecognizeId);

                FaceRecognitionStatus statusCreated = await _faceRecognitionStatusRepository.CreateFaceRecognitionStatus(status);


                if (statusCreated.Id != 0)
                {
                    response = new ResponseApi<FaceRecognitionStatusDto>() { Status = true, Message = "Face Recognition Status Created", Value = _mapper.Map<FaceRecognitionStatusDto>(statusCreated) };
                }
                else
                {
                    response = new ResponseApi<FaceRecognitionStatusDto>() { Status = false, Message = "Could not create" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<FaceRecognitionStatusDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        //[HttpGet]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<FaceRecognitionStatus>))]
        //public IActionResult GetFaceRecognitionStatuses()
        //{
        //    var statuses = _mapper.Map<List<FaceRecognitionStatusDto>>(_faceRecognitionStatusRepository.GetFaceRecognitionStatuses());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(statuses);
        //}

        //[HttpGet("{id}/person-faces")]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<FaceRecognitionStatus>))]
        //[ProducesResponseType(400)]
        //public IActionResult GetFaceRecognitionStatusesByPersonId(int id)
        //{
        //    var statuses = _mapper.Map<List<FaceRecognitionStatusDto>>(_faceRecognitionStatusRepository.GetFaceRecognitionStatusesByPersonId(id));

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(statuses);
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(200, Type = typeof(FaceRecognitionStatus))]
        //[ProducesResponseType(400)]
        //public IActionResult GetFaceRecognitionStatusById(int id)
        //{
        //    if (!_faceRecognitionStatusRepository.FaceRecognitionStatusExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var status = _mapper.Map<FaceRecognitionStatusDto>(_faceRecognitionStatusRepository.GetFaceRecognitionStatusById(id));

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(status);
        //}

        //[HttpGet("face-to-recognize/{id}")]
        //[ProducesResponseType(200, Type = typeof(FaceRecognitionStatus))]
        //[ProducesResponseType(400)]
        //public IActionResult GetFaceRecognitionStatusByFaceToRecognizeId(int id)
        //{
        //    if (!_faceRecognitionStatusRepository.FaceRecognitionStatusExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var status = _mapper.Map<FaceRecognitionStatusDto>(_faceRecognitionStatusRepository.GetFaceRecognitionStatusByFaceToRecognizeId(id));

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(status);
        //}


        //[HttpPut("{id}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public IActionResult UpdateFaceRecognitionStatus(int id, [FromBody] FaceRecognitionStatusDto updatedFaceRecognitionStatus)
        //{
        //    if (updatedFaceRecognitionStatus == null)
        //        return BadRequest(ModelState);

        //    if (id != updatedFaceRecognitionStatus.Id)
        //        return BadRequest(ModelState);

        //    if (!_faceRecognitionStatusRepository.FaceRecognitionStatusExists(id))
        //        return NotFound();

        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    var faceMap = _mapper.Map<FaceRecognitionStatus>(updatedFaceRecognitionStatus);

        //    if (!_faceRecognitionStatusRepository.UpdateFaceRecognitionStatus(faceMap))
        //    {
        //        ModelState.AddModelError("", "Something went wrong status");
        //        return StatusCode(500, ModelState);
        //    }

        //    return Ok("Successfully updated");
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public IActionResult DeleteFaceRecognitionStatus(int id)
        //{
        //    if (!_faceRecognitionStatusRepository.FaceRecognitionStatusExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var status = _faceRecognitionStatusRepository.GetFaceRecognitionStatusById(id);

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    if (!_faceRecognitionStatusRepository.DeleteFaceRecognitionStatus(status))
        //    {
        //        ModelState.AddModelError("", "Something went wrong deleting status");
        //    }

        //    return Ok("Successfully deleted");
        //}
    }
}
