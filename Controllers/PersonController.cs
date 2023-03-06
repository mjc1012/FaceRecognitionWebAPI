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
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public PersonController(IUnitOfWork uow, IPersonRepository personRepository, IMapper mapper)
        {
            _uow = uow;
            _personRepository = personRepository;
            _mapper= mapper;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] PersonDto request)
        {
            ResponseApi<TokenDto> response;
            try
            {
                Person person = await _uow.personRepository.GetPerson(request.ValidIdNumber);

                if (person == null || !PasswordHasher.VerifyPassword(request.Password, person.Password))
                {
                    response = new ResponseApi<TokenDto>() { Status = false, Message = "User Not Found" };
                }
                else
                {
                    string accessToken = _uow.authenticationRepository.CreateJwt(person);
                    string refreshToken = await _uow.authenticationRepository.CreateRefreshToken();
                    DateTime refreshTokenExpiryTime = DateTime.Now.AddDays(5);
                    Person personWithToken = await _uow.authenticationRepository.saveTokens(person, accessToken, refreshToken, refreshTokenExpiryTime);
                    TokenDto token = new TokenDto()
                    {
                        AccessToken = personWithToken.Token,
                        RefreshToken = personWithToken.RefreshToken
                    };
                    response = new ResponseApi<TokenDto>() { Status = true, Message = "User Found", Value = token };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<TokenDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            ResponseApi<TokenDto> response;
            try
            {
                var principal = _uow.authenticationRepository.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
                var idNumber = principal.Identity.Name;
                Person person = await _uow.personRepository.GetPerson(idNumber);
                if (person == null || person.RefreshToken != tokenDto.RefreshToken || person.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    response = new ResponseApi<TokenDto>() { Status = false, Message = "Invalid Request" };
                }
                else
                {
                    string newAccessToken = _uow.authenticationRepository.CreateJwt(person);
                    string newRefreshToken = await _uow.authenticationRepository.CreateRefreshToken();
                    Person personWithToken = await _uow.authenticationRepository.saveTokens(person, newAccessToken, newRefreshToken);
                    TokenDto token = new TokenDto()
                    {
                        AccessToken = personWithToken.Token,
                        RefreshToken = personWithToken.RefreshToken
                    };
                    response = new ResponseApi<TokenDto>() { Status = true, Message = "User Found", Value = token };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<TokenDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPersons()
        {
            ResponseApi<List<PersonDto>> response;
            try
            {
                List<PersonDto> people = _mapper.Map<List<PersonDto>>(await _personRepository.GetPeople());

                if (people.Count > 0)
                {
                    response = new ResponseApi<List<PersonDto>>() { Status = true, Message = "Got All Person", Value = people };
                }
                else
                {
                    response = new ResponseApi<List<PersonDto>>() { Status = false, Message = "No data" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<List<PersonDto>>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }

        [HttpGet("{validIdNumber}")]
        public async Task<IActionResult> GetPerson(string validIdNumber)
        {
            ResponseApi<PersonDto> response;
            try
            {

                Person person = await _personRepository.GetPerson(validIdNumber);

                if (person != null)
                {
                    response = new ResponseApi<PersonDto>() { Status = true, Message = "Person Found", Value = _mapper.Map<PersonDto>(person) };
                }
                else
                {
                    response = new ResponseApi<PersonDto>() { Status = false, Message = "Could not find Person" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto request)
        {
            ResponseApi<PersonDto> response;
            try
            {
                request.Password = PasswordHasher.HashPassword(request.FirstName + "Alliance" + request.LastName + "@123");
                Person person = _mapper.Map<Person>(request);

                Person personCreated = await _personRepository.CreatePerson(person);

                if (personCreated.Id != 0)
                {
                    response = new ResponseApi<PersonDto>() { Status = true, Message = "Person Created", Value = _mapper.Map<PersonDto>(personCreated) };
                }
                else
                {
                    response = new ResponseApi<PersonDto>() { Status = false, Message = "Could not create Person" };
                }
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePerson([FromBody] PersonDto request)
        {
            ResponseApi<PersonDto> response;
            try
            {
                Person person = _mapper.Map<Person>(request);
                Person personEdited = await _personRepository.UpdatePerson(person);

                response = new ResponseApi<PersonDto>() { Status = true, Message = "Person Updated", Value = _mapper.Map<PersonDto>(personEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpPut("update-using-valid-id-number")]
        public async Task<IActionResult> UpdatePersonUsingValidIdNumber([FromBody] PersonDto request)
        {
            ResponseApi<PersonDto> response;
            try
            {
                Person personFound = await _personRepository.GetPerson(request.ValidIdNumber);
                Person person = _mapper.Map<Person>(request);
                person.Id = personFound.Id;
                _personRepository.DetachPerson(personFound);
                Person personEdited = await _personRepository.UpdatePerson(person);

                response = new ResponseApi<PersonDto>() { Status = true, Message = "Person Updated", Value = _mapper.Map<PersonDto>(personEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            ResponseApi<bool> response;
            try
            {

                Person employee = await _personRepository.GetPerson(id);
                bool deleted = await _personRepository.DeletePerson(employee);

                if (deleted)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Person Deleted" };
                }
                else
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Could not delete Person" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [Authorize]
        [HttpDelete("use-valid-id-number/{validIdNumber}")]
        public async Task<IActionResult> DeletePersonWithValidIdNumber(string validIdNumber)
        {
            ResponseApi<bool> response;
            try
            {

                Person employee = await _personRepository.GetPerson(validIdNumber);
                bool deleted = await _personRepository.DeletePerson(employee);

                if (deleted)
                {
                    response = new ResponseApi<bool>() { Status = true, Message = "Person Deleted" };
                }
                else
                {
                    response = new ResponseApi<bool>() { Status = false, Message = "Could not delete Person" };
                }

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<bool>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PersonDto request)
        {
            ResponseApi<PersonDto> response;
            try
            {
                string passwordError = _uow.authenticationRepository.CheckPasswordStrength(request.Password);
                if (passwordError != "")
                {
                    response = new ResponseApi<PersonDto>() { Status = false, Message = passwordError };
                    return StatusCode(StatusCodes.Status200OK, response);
                }

                Person oldEmployee = await _uow.personRepository.GetPerson(request.ValidIdNumber);
                oldEmployee.Password = PasswordHasher.HashPassword(request.Password);
                _uow.personRepository.DetachPerson(oldEmployee);
                Person employeeEdited = await _uow.personRepository.UpdatePerson(oldEmployee);

                response = new ResponseApi<PersonDto>() { Status = true, Message = "Password Updated", Value = _mapper.Map<PersonDto>(employeeEdited) };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response = new ResponseApi<PersonDto>() { Status = false, Message = ex.Message };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        //[HttpGet("{id}")]
        //[ProducesResponseType(200, Type = typeof(Person))]
        //[ProducesResponseType(400)]
        //public IActionResult GetPersonById(int id)
        //{
        //    if(!_personRepository.PersonExists(id))
        //    {
        //        return NotFound();
        //    }

        //    var person = _mapper.Map<PersonDto>(_personRepository.GetPersonById(id));

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(person);
        //}



    }
}
