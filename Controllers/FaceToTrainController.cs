using AutoMapper;
using FaceRecognitionWebAPI.Dto;
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

        [HttpGet("{pairId}/person-faces")]
        public async Task<IActionResult> GetFacesToTrain(int pairId)
        {
            ResponseDto<List<FaceToTrainDto>> response;
            try
            {
                List<FaceToTrainDto> faces = _mapper.Map<List<FaceToTrainDto>>(await _uow.faceToTrainRepository.GetFacesToTrain(pairId));

                if (faces.Count > 0)
                {
                    response = new ResponseDto<List<FaceToTrainDto>>() { Status = true, Message = "Got User's Faces To Train", Value = faces };
                }
                else
                {
                    response = new ResponseDto<List<FaceToTrainDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<FaceToTrainDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{pairId}/missing-expression")]
        public async Task<IActionResult> GetMissingFaceExpression(int pairId)
        {
            ResponseDto<FaceExpressionDto> response;
            try
            {

                List<FaceExpression> faceExpressions = await _uow.faceExpressionRepository.GetFaceExpressions();
                FaceExpressionDto missingExpression = _mapper.Map<FaceExpressionDto>(await _uow.faceToTrainRepository.GetMissingFaceExpressionOfPerson(pairId, faceExpressions));

                if (missingExpression != null)
                {
                    response = new ResponseDto<FaceExpressionDto>() { Status = true, Message = "Got Missing Face Expression", Value = missingExpression };
                }
                else
                {
                    response = new ResponseDto<FaceExpressionDto>() { Status = false, Message = "no data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceExpressionDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateAndAugmentFaceToTrain([FromBody] FaceToTrainDto request)
        {

            ResponseDto<FaceToTrainDto> response;
            try
            {
                Person person = await _uow.personRepository.GetPersonByPairId(request.PairId);
                request.ImageFile = _uow.imageService.SaveImage(request.Base64String, person.Id);
                var faceMap = _mapper.Map<FaceToTrain>(request);
                faceMap.Person = person;
                faceMap.FaceExpression = await _uow.faceExpressionRepository.GetFaceExpression(request.FaceExpressionId);

                FaceToTrain faceCreated = await _uow.faceToTrainRepository.CreateFaceToTrain(faceMap);

                await _uow.imageAugmentationService.RunImageAugmentation(faceCreated);

                if (faceCreated.Id != 0)
                {
                    response = new ResponseDto<FaceToTrainDto>() { Status = true, Message = "Face To Train Created", Value = _mapper.Map<FaceToTrainDto>(faceMap) };
                }
                else
                {
                    response = new ResponseDto<FaceToTrainDto>() { Status = false, Message = "Face To Train Not Created" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<FaceToTrainDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFaceToTrain(int id)
        {

            ResponseDto<bool> response;
            try
            {

                FaceToTrain face = await _uow.faceToTrainRepository.GetFaceToTrain(id);
                List<AugmentedFace> augmentedFaces = await _uow.augmentedFaceRepository.GetAugmentedFaces(face.Id);
                if (!_uow.imageService.DeleteImage(face, augmentedFaces))
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool deleted = await _uow.faceToTrainRepository.DeleteFaceToTrain(face);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Face To Train Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Face To Train Not Deleted" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
