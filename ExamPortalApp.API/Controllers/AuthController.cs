using AutoMapper;
using ExamPortalApp.Contracts.Data.Dtos;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExamPortalApp.Infrastructure.Exceptions;
using ExamPortalApp.Infrastructure;

namespace ExamPortalApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthRepository authRepository,IStudentRepository studentRepository,  IMapper mapper) : ControllerBase
    {
        private readonly IAuthRepository _auth = authRepository;
        private readonly IStudentRepository _studRepo = studentRepository;
        private readonly IMapper _mapper = mapper;

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            try
            {
                //var user = await _auth.LoginAsync(loginModel) ?? throw new BadRequestException("Login credentials are incorrect");
                var user = await _auth.LoginAsync(loginModel) ?? throw new InvalidPasswordException(); 
                return Ok(user);
            }
            catch (LicenseExipredException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch(NotApprovedException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch(InvalidUserNameException ex)
            {
                return StatusCode(500, ex.Message);
            }

            catch(InvalidPasswordException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch(InvalidCrdentialsException  ex){
                return StatusCode(500, ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Please Contact the Administrator");
            }
        }

        [AllowAnonymous]
        [HttpPost("LoginStudent")]
        public async Task<ActionResult> LoginStudent(StudentLoginModel loginModel)
        {
            try
            {
                var user = await _auth.LoginStudentAsync(loginModel);
                return Ok(user);
            }
            catch (NotApprovedException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(InvalidCrdentialsException ex){
                return StatusCode(500, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Please Contact the Administrator");
            }
            
        }

        [AllowAnonymous]
        [HttpPost("LoginStudentTest")]

        public async Task<ActionResult> LoginStudentTest(StudentTestLoginModel loginTestModel)
        {
            try
            {
                var user = await _auth.LoginStudentTestAsync(loginTestModel);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("LoginStudentExternal")]
        public async Task<ActionResult> LoginStudentExternal(string email)
        {
            try
            {
                var user = await _auth.LoginStudentAsync(email);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            try
            {
                var user = await _auth.RegisterAsync(model);
                var result = _mapper.Map<UserDto>(user);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

         [HttpPost("PasswordMigration")]
        public async Task<ActionResult> PasswordMigration()
        {
            try
            {
               await _studRepo.PasswordMigration();
               return Ok("");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
