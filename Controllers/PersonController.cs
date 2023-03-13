using AutoMapper;
using FaceRecognitionWebAPI.Dto;
using FaceRecognitionWebAPI.Helper;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Models;
using FaceRecognitionWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] PersonDto request)
        {
            ResponseDto<TokenDto> response;
            try
            {
                Person person = await _uow.personRepository.GetPerson(request.ValidIdNumber);

                if (person == null || !PasswordHasher.VerifyPassword(request.Password, person.Password))
                {
                    response = new ResponseDto<TokenDto>() { Status = false, Message = "User Not Found" };
                }
                else
                {
                    string accessToken = _uow.authenticationRepository.CreateJwt(person);
                    string refreshToken = await _uow.authenticationRepository.CreateRefreshToken();
                    DateTime refreshTokenExpiryTime = DateTime.Now.AddDays(5);
                    Person personWithToken = await _uow.authenticationRepository.saveTokens(person, accessToken, refreshToken, refreshTokenExpiryTime);
                    TokenDto token = new()
                    {
                        AccessToken = personWithToken.AccessToken,
                        RefreshToken = personWithToken.RefreshToken
                    };
                    response = new ResponseDto<TokenDto>() { Status = true, Message = "User Found", Value = token };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<TokenDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            ResponseDto<TokenDto> response;
            try
            {
                var principal = _uow.authenticationRepository.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
                var idNumber = principal.Identity.Name;
                Person person = await _uow.personRepository.GetPerson(idNumber);
                if (person == null || person.RefreshToken != tokenDto.RefreshToken || person.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    response = new ResponseDto<TokenDto>() { Status = false, Message = "Invalid Request" };
                }
                else
                {
                    string newAccessToken = _uow.authenticationRepository.CreateJwt(person);
                    string newRefreshToken = await _uow.authenticationRepository.CreateRefreshToken();
                    Person personWithToken = await _uow.authenticationRepository.saveTokens(person, newAccessToken, newRefreshToken);
                    TokenDto token = new()
                    {
                        AccessToken = personWithToken.AccessToken,
                        RefreshToken = personWithToken.RefreshToken
                    };
                    response = new ResponseDto<TokenDto>() { Status = true, Message = "User Found", Value = token };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<TokenDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPersons()
        {
            ResponseDto<List<PersonDto>> response;
            try
            {
                List<PersonDto> people = _mapper.Map<List<PersonDto>>(await _uow.personRepository.GetPeople());

                if (people.Count > 0)
                {
                    response = new ResponseDto<List<PersonDto>>() { Status = true, Message = "Got All Person", Value = people };
                }
                else
                {
                    response = new ResponseDto<List<PersonDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<List<PersonDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [Authorize]
        [HttpGet("{validIdNumber}")]
        public async Task<IActionResult> GetPerson(string validIdNumber)
        {
            ResponseDto<PersonDto> response;
            try
            {

                Person person = await _uow.personRepository.GetPerson(validIdNumber);

                if (person != null)
                {
                    response = new ResponseDto<PersonDto>() { Status = true, Message = "Person Found", Value = _mapper.Map<PersonDto>(person) };
                }
                else
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "Could not find Person" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto request)
        {
            ResponseDto<PersonDto> response;
            try
            {
                request.Password = PasswordHasher.HashPassword(request.FirstName.Replace(" ", String.Empty) + "Alliance" + request.LastName + "@123");
                Person person = _mapper.Map<Person>(request);

                Person personCreated = await _uow.personRepository.CreatePerson(person);

                if (personCreated.Id != 0)
                {
                    response = new ResponseDto<PersonDto>() { Status = true, Message = "Person Created", Value = _mapper.Map<PersonDto>(personCreated) };
                }
                else
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = "Could not create Person" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePerson([FromBody] PersonDto request)
        {
            ResponseDto<PersonDto> response;
            try
            {
                Person person = _mapper.Map<Person>(request);
                Person personEdited = await _uow.personRepository.UpdatePerson(person);

                response = new ResponseDto<PersonDto>() { Status = true, Message = "Person Updated", Value = _mapper.Map<PersonDto>(personEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut("update-using-valid-id-number")]
        public async Task<IActionResult> UpdatePersonUsingValidIdNumber([FromBody] PersonDto request)
        {
            ResponseDto<PersonDto> response;
            try
            {
                Person personFound = await _uow.personRepository.GetPerson(request.ValidIdNumber);
                Person person = _mapper.Map<Person>(request);
                person.Id = personFound.Id;
                _uow.personRepository.DetachPerson(personFound);
                Person personEdited = await _uow.personRepository.UpdatePerson(person);

                response = new ResponseDto<PersonDto>() { Status = true, Message = "Person Updated", Value = _mapper.Map<PersonDto>(personEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            ResponseDto<bool> response;
            try
            {

                Person employee = await _uow.personRepository.GetPerson(id);
                bool deleted = await _uow.personRepository.DeletePerson(employee);

                if (deleted)
                {
                    _uow.imageService.DeleteFolder(id);
                    response = new ResponseDto<bool>() { Status = true, Message = "Person Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Could not delete Person" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut("deletePeople")]
        public async Task<IActionResult> DeletePeople([FromBody] DeleteRangeDto deleteRange)
        {
            ResponseDto<bool> response;
            try
            {

                List<Person> people = _mapper.Map<List<Person>>(await _uow.personRepository.GetPeople(deleteRange.IdNumbers));
                bool deleted = await _uow.personRepository.DeletePeople(people);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Employee Deleted" };
                }
                else
                {
                    response = new ResponseDto<bool>() { Status = false, Message = "Could not delete" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpDelete("use-valid-id-number/{validIdNumber}")]
        public async Task<IActionResult> DeletePersonWithValidIdNumber(string validIdNumber)
        {
            ResponseDto<bool> response;
            try
            {

                Person employee = await _uow.personRepository.GetPerson(validIdNumber);
                bool deleted = await _uow.personRepository.DeletePerson(employee);

                if (deleted)
                {
                    response = new ResponseDto<bool>() { Status = true, Message = "Person Deleted" };
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

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PersonDto request)
        {
            ResponseDto<PersonDto> response;
            try
            {
                string passwordError = _uow.authenticationRepository.CheckPasswordStrength(request.Password);
                if (passwordError != "")
                {
                    response = new ResponseDto<PersonDto>() { Status = false, Message = passwordError };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                Person oldEmployee = await _uow.personRepository.GetPerson(request.ValidIdNumber);
                oldEmployee.Password = PasswordHasher.HashPassword(request.Password);
                _uow.personRepository.DetachPerson(oldEmployee);
                Person employeeEdited = await _uow.personRepository.UpdatePerson(oldEmployee);

                response = new ResponseDto<PersonDto>() { Status = true, Message = "Password Updated", Value = _mapper.Map<PersonDto>(employeeEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseDto<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



    }
}
