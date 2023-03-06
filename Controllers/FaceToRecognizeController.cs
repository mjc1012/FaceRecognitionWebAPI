using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
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
        private readonly IFaceToRecognizeRepository _faceToRecognizeRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        public FaceToRecognizeController(IFaceToRecognizeRepository faceToRecognizeRepository, IImageService imageService, IMapper mapper)
        {
            _faceToRecognizeRepository = faceToRecognizeRepository;
            _imageService = imageService;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> CreateFaceToRecognize([FromBody] FaceToRecognizeDto request)
        {
            ResponseApi<FaceToRecognizeDto> response;
            try
            {
                request.ImageFile = _imageService.SaveImage(request.Base64String, -1);
                FaceToRecognize face = _mapper.Map<FaceToRecognize>(request);

                FaceToRecognize faceCreated = await _faceToRecognizeRepository.CreateFaceToRecognize(face);

                if (faceCreated.Id != 0)
                {
                    response = new ResponseApi<FaceToRecognizeDto>() { Status = true, Message = "Face To Recognize Created", Value = _mapper.Map<FaceToRecognizeDto>(faceCreated) };
                }
                else
                {
                    response = new ResponseApi<FaceToRecognizeDto>() { Status = false, Message = "Could not create" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<FaceToRecognizeDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        //[HttpGet]
        //[ProducesResponseType(200, Type = typeof(IEnumerable<FaceToRecognize>))]
        //public IActionResult GetAugmentedFaces()
        //{
        //    var faces = _mapper.Map<List<FaceToRecognizeDto>>(_faceToRecognizeRepository.GetFacesToRecognize());

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(faces);
        //}

        //[HttpGet("{id}")]
        //[ProducesResponseType(200, Type = typeof(FaceToRecognize))]
        //[ProducesResponseType(400)]
        //public IActionResult GetFaceToRecognizeById(int id)
        //{
        //    if (!_faceToRecognizeRepository.FaceToRecognizeExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var face = _mapper.Map<FaceToRecognizeDto>(_faceToRecognizeRepository.GetFaceToRecognizeById(id));

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(face);
        //}

        //[HttpPut("{id}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public IActionResult UpdateFaceToRecognize(int id, [FromBody] FaceToRecognizeDto updatedFaceToRecognize)
        //{
        //    if (updatedFaceToRecognize == null)
        //        return BadRequest(ModelState);

        //    if (id != updatedFaceToRecognize.Id)
        //        return BadRequest(ModelState);

        //    if (!_faceToRecognizeRepository.FaceToRecognizeExists(id))
        //        return NotFound();

        //    var face = _faceToRecognizeRepository.GetFaceToRecognizeById(id);
        //    var imageName = face.ImageName;
        //    _faceToRecognizeRepository.DetachTracking(face);

        //    if (!ModelState.IsValid || !_imageService.DeleteImage(face))
        //        return BadRequest();

        //    var imageResult = _imageService.SaveImage(updatedFaceToRecognize.Base64String, -1);
        //    if (imageResult.Item1 == 1)
        //    {
        //        updatedFaceToRecognize.ImageName = imageResult.Item2; // getting name of image
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var faceMap = _mapper.Map<FaceToRecognize>(updatedFaceToRecognize);

        //    if (!_faceToRecognizeRepository.UpdateFaceToRecognize(faceMap))
        //    {
        //        ModelState.AddModelError("", "Something went wrong updating face");
        //        return StatusCode(500, ModelState);
        //    }

        //    return Ok("Successfully updated");
        //}

        //[HttpDelete("{id}")]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public IActionResult DeleteFaceToRecognize(int id)
        //{
        //    if (!_faceToRecognizeRepository.FaceToRecognizeExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var face = _faceToRecognizeRepository.GetFaceToRecognizeById(id);
        //    var imageName = face.ImageName;

        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    if (!_faceToRecognizeRepository.DeleteFaceToRecognize(face))
        //    {
        //        ModelState.AddModelError("", "Something went wrong deleting face");
        //    }


        //    if (!_imageService.DeleteImage(face))
        //        return BadRequest();

        //    return Ok("Successfully deleted");
        //}
    }
}
