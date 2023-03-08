using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FaceRecognitionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceToTrainController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public FaceToTrainController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFacesToTrain()
        {
            ResponseApi<List<FaceToTrainDto>> response;
            try
            {
                List<FaceToTrainDto> faces = _mapper.Map<List<FaceToTrainDto>>(await _uow.faceToTrainRepository.GetFacesToTrain());

                if (faces.Count > 0)
                {
                    response = new ResponseApi<List<FaceToTrainDto>>() { Status = true, Message = "Got All Faces To Train", Value = faces };
                }
                else
                {
                    response = new ResponseApi<List<FaceToTrainDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<FaceToTrainDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpGet("{personId}/person-faces")]
        public async Task<IActionResult> GetFacesToTrain(int personId)
        {
            ResponseApi<List<FaceToTrainDto>> response;
            try
            {
                List<FaceToTrainDto> faces = _mapper.Map<List<FaceToTrainDto>>(await _uow.faceToTrainRepository.GetFacesToTrain(personId));

                if (faces.Count > 0)
                {
                    response = new ResponseApi<List<FaceToTrainDto>>() { Status = true, Message = "Got Person's Faces To Train", Value = faces };
                }
                else
                {
                    response = new ResponseApi<List<FaceToTrainDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<FaceToTrainDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [Authorize]
        [HttpGet("{id}/missing-expression")]
        public async Task<IActionResult> GetMissingFaceExpression(int id)
        {
            ResponseApi<FaceExpressionDto> response;
            try
            {

                List<FaceExpression> faceExpressions = await _uow.faceExpressionRepository.GetFaceExpressions();
                FaceExpressionDto missingExpression = _mapper.Map<FaceExpressionDto>(await _uow.faceToTrainRepository.GetMissingFaceExpressionOfPerson(id, faceExpressions));

                if (missingExpression != null)
                {
                    response = new ResponseApi<FaceExpressionDto>() { Status = true, Message = "Got Missing Face Expression", Value = missingExpression };
                }
                else
                {
                    response = new ResponseApi<FaceExpressionDto>() { Status = false, Message = "no data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<FaceExpressionDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAndAugmentFaceToTrain([FromBody] FaceToTrainDto request)
        {

            ResponseApi<FaceToTrainDto> response;
            try
            {
                request.ImageFile = _uow.imageService.SaveImage(request.Base64String, request.PersonId);
                var faceMap = _mapper.Map<FaceToTrain>(request);
                faceMap.Person = await _uow.personRepository.GetPerson(request.PersonId);
                faceMap.FaceExpression = await _uow.faceExpressionRepository.GetFaceExpression(request.FaceExpressionId);

                FaceToTrain faceCreated = await _uow.faceToTrainRepository.CreateFaceToTrain(faceMap);

                await _uow.imageAugmentationService.RunImageAugmentation(faceCreated);

                if (faceCreated.Id != 0)
                {
                    response = new ResponseApi<FaceToTrainDto>() { Status = true, Message = "Face To Train Created", Value = _mapper.Map<FaceToTrainDto>(faceMap) };
                }
                else
                {
                    response = new ResponseApi<FaceToTrainDto>() { Status = false, Message = "Could not create" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<FaceToTrainDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaceToTrain(int id)
        {

            ResponseApi<bool> response;
            try
            {

                FaceToTrain face = await _uow.faceToTrainRepository.GetFaceToTrain(id);
                List<AugmentedFace> augmentedFaces = await _uow.augmentedFaceRepository.GetAugmentedFaces(face.Id);
                if (!(await _uow.imageService.DeleteImage(face, augmentedFaces)))
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Could not delete" };
                }
                bool deleted = await _uow.faceToTrainRepository.DeleteFaceToTrain(face);

                if (deleted)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Face To Train Deleted" };
                }
                else
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Could not delete" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
