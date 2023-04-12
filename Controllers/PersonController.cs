using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FaceRecognitionWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public PersonController(IUnitOfWork uow,  IMapper mapper)
        {
            _uow = uow;
            _mapper= mapper;
        }

        [HttpGet("{pairId}")]
        public async Task<IActionResult> GetPerson(int pairId)
        {
            ResponseDto<PersonDto> response;
            try
            {

                Person person = await _uow.personRepository.GetPersonByPairId(pairId);

                if (person != null)
                {
                    response = new ResponseDto<PersonDto>() { Status = true, Message = "User Found", Value = _mapper.Map<PersonDto>(person) };
                }
                else
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "User Not Found" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto request)
        {
            ResponseDto<PersonDto> response;
            try
            {
                Person checkPerson = await _uow.personRepository.GetPersonByPairId(request.PairId);

                if (checkPerson != null)
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "Pair Id Already Exist" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                Person person = _mapper.Map<Person>(request);

                Person personCreated = await _uow.personRepository.CreatePerson(person);

                if (personCreated.Id != 0 && personCreated != null)
                {
                    response = new ResponseDto<PersonDto>() { Status = true, Message = "User Created", Value = _mapper.Map<PersonDto>(personCreated) };
                }
                else
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "User Not Created" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePerson([FromBody] PersonDto request)
        {
            ResponseDto<PersonDto> response;
            try
            {
                Person personFound = await _uow.personRepository.GetPersonByPairId(request.PairId);
                Person person = _mapper.Map<Person>(request);
                person.Id = personFound.Id;
                _uow.personRepository.DetachPerson(personFound);
                Person personEdited = await _uow.personRepository.UpdatePerson(person);

                response = new ResponseDto<PersonDto>() { Status = true, Message = "User Updated", Value = _mapper.Map<PersonDto>(personEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut("delete-people")]
        public async Task<IActionResult> DeletePeople([FromBody] DeleteRangeDto deleteRange)
        {
            ResponseDto<bool> response;
            try
            {

                List<Person> people = _mapper.Map<List<Person>>(await _uow.personRepository.GetPeople(deleteRange.Ids));
                bool deleted = await _uow.personRepository.DeletePeople(people);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Users Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Users Not Deleted" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{pairId}")]
        public async Task<IActionResult> DeletePerson(int pairId)
        {
            ResponseDto<bool> response;
            try
            {

                Person person = await _uow.personRepository.GetPersonByPairId(pairId);
                if (!_uow.imageService.DeleteFolder(person.Id))
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Something went wrong" };
                    return StatusCode(StatusCodes.Status200OK, response);
                }
                bool deleted = await _uow.personRepository.DeletePerson(person);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "User Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Could not delete Person" };
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
