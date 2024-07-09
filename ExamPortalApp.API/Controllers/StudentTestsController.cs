﻿using AutoMapper;
using ExamPortalApp.Contracts.Data.Dtos;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamPortalApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentTestsController(IStudentTestRepository studentTestRepository, IMapper mapper) : CrudControllerBase<StudentTestDTO, StudentTest>(mapper)
    {
        private readonly IStudentTestRepository _studentTestRepository = studentTestRepository;

        [HttpPost("accept-disclaimer")]
        public async Task<ActionResult> AcceptDisclaimer(int testId, int studentId, bool isDisclaimerAccepted)
        {
            try
            {
                var result = await _studentTestRepository.AcceptDisclaimer(testId, studentId, isDisclaimerAccepted);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("save-answers-interval")]
        public async Task<ActionResult<StudentTestAnswers>> SaveAnswersInterval(StudentTestAnswerModel studentTestAnswers)
        {
            try
            {
                var result = await _studentTestRepository.SaveAnswersInterval(studentTestAnswers);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("finish-test")]
        public async Task<ActionResult<StudentTestAnswers>> FinishTest(StudentTestAnswers studentTestAnswers)
        {
            try
            {
                var result = await _studentTestRepository.FinishTest(studentTestAnswers);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpDelete("{id}")]
        public override async Task<ActionResult<StudentTestDTO>> Delete(int id)
        {
            try
            {
                var response = await _studentTestRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public override async Task<ActionResult<IEnumerable<StudentTestDTO>>> Get()
        {
            try
            {
                var testSecurityLevels = await _studentTestRepository.GetAllAsync();
                var result = _mapper.Map<IEnumerable<TestSecurityLevelDto>>(testSecurityLevels);

                return Ok(testSecurityLevels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public override async Task<ActionResult<StudentTestDTO>> Get(int id)
        {
            try
            {
                var testSecurityLevel = await _studentTestRepository.GetAsync(id);
                var result = _mapper.Map<TestSecurityLevelDto>(testSecurityLevel);

                return Ok(testSecurityLevel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /* [HttpPost]
         public async JsonResult GetWordDocument(int fileId)
         {
             byte[] bytes;
             string fileName, contentType;
             var docs = await _studentTestRepository.GetUserAnswerDocumentAsync(studentId, testId);

             var doc = docs?.FirstOrDefault();

             if (doc?.TestDocument is null) //throw new Exception("No test document found");
             {
                 try
                 {
                     byte[] bytes;
                     bytes = new byte[0];
                     return bytes;
                 }
                 catch (Exception ex)
                 { }
             }
             return Json(new { FileName = docs.fileName ?? "", ContentType = contentType, Data = bytes });
         }*/

        /* var docs = await GetUserAnswerDocumentAsync(studentId, testId);
         var doc = docs?.FirstOrDefault();

             if (doc?.TestDocument is null) //throw new Exception("No test document found");
             {
                 try
                 {
                     byte[] bytes;
         bytes = new byte[0];
                     return bytes;
                 }
                 catch (Exception ex)
                 { }
             }
             return doc.TestDocument;*/

        /*
           Fix: Object of type 'System.DateTime' cannot be converted to type 'System.String'.
        */
        [AllowAnonymous]
        [HttpGet("get-students-testdetails")]
        public async Task<ActionResult<StudentTestAnswers>> GetStudentTestDetails(int testId, int studentId)
        {
            try
            {
                var result = await _studentTestRepository.GetStudentTestDetails(testId, studentId);
                //result.
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        
        [HttpPost]
        public override async Task<ActionResult<StudentTestDTO>> Post(StudentTest studentTest)
        {
            try
            {
                var response = await _studentTestRepository.AddAsync(studentTest);
                var result = _mapper.Map<StudentTestDTO>(response);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public override Task<ActionResult<StudentTestDTO>> Put(int id, StudentTest entity)
        {
            throw new NotImplementedException();
        }
    }
}

