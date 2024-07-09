using AutoMapper;
using ExamPortalApp.Contracts.Data.Dtos;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ExamPortalApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BulkImportController : CrudControllerBase<StudentTestDTO, StudentTest>
    {
        private readonly IWebHostEnvironment _env;
        private readonly IBulkImportRepository _bulkImportRepository;
        //private string[] fileNames;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;


        public BulkImportController(IBulkImportRepository bulkImportRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IWebHostEnvironment env, IConfiguration configuration) : base(mapper)
        {
            _bulkImportRepository = bulkImportRepository;
            _contextAccessor = contextAccessor;
            _env = env;
            _configuration = configuration;

        }

        [HttpDelete("{id}")]
        public override async Task<ActionResult<StudentTestDTO>> Delete(int id)
        {
            try
            {
                var response = await _bulkImportRepository.DeleteAsync(id);

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
                var testSecurityLevels = await _bulkImportRepository.GetAllAsync();
                var result = _mapper.Map<IEnumerable<StudentTestDTO>>(testSecurityLevels);

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
                var testSecurityLevel = await _bulkImportRepository.GetAsync(id);
                var result = _mapper.Map<StudentTestDTO>(testSecurityLevel);

                return Ok(testSecurityLevel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public override async Task<ActionResult<StudentTestDTO>> Post(StudentTest StudentTest)
        {
            try
            {
                var response = await _bulkImportRepository.AddAsync(StudentTest);
                var result = _mapper.Map<StudentTestDTO>(response);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        [HttpPost("add-bulkImportFiles")]
        // public async  Task<ActionResult> BulkImportFiles(List<IFormFile> files)
        public async Task<ActionResult> BulkImportFiles(List<IFormFile> files)
        {
            try
            {
                string _batchGuid = Guid.NewGuid().ToString();
                bool proccesFirstFile = await _bulkImportRepository.ImportFile1(Request.Form.Files[0].OpenReadStream(), _batchGuid);

                if (!proccesFirstFile) return BadRequest();

                bool proccesSecondFile = await _bulkImportRepository.ImportFile2(Request.Form.Files[1].OpenReadStream(), _batchGuid);
                if (proccesFirstFile && proccesSecondFile)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /*    [HttpGet("download")]
     public IActionResult DownloadExcel()
     {
         var filePath = Path.Combine(_env.WebRootPath, "Templates", "my-excel-file.xlsx");
         var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

         return PhysicalFile(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "my-excel-file.xlsx");
     }
  */
        [AllowAnonymous]
        [HttpGet("download-student-template")]
        public async Task<ActionResult> DownloadStudentTemplate()

        {
            ///var filePath = contentRootpath + $@"/templates/StudentsBulkImport.xlsx";
            var uploads = Path.Combine(_env.WebRootPath, "Templates");
            var filePath = Path.Combine(uploads, "StudentsBulkImport.xlsx");
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), "StudentsBulkImport.xlsx");
        }

        [AllowAnonymous]
        [HttpGet("download-subjectsectors-template")]
        public async Task<ActionResult> DownloadSubjectSectorsTemplate()
        {
            ///var filePath = contentRootpath + $@"/templates/StudentsBulkImport.xlsx";
            var uploads = Path.Combine(_env.WebRootPath, "Templates");
            var filePath = Path.Combine(uploads, "SubjectSectorsBulkImport.xlsx");
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), "SubjectSectorsBulkImport.xlsx");
        }


        private static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        [AllowAnonymous]
        //[DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        [HttpPost("insert-bulkImportFileData")]
        public async Task<ActionResult> InsertBulkImportFileData(BatchIdLinker linker)
        {
            try
            {

                bool insertFromTempStorage = await _bulkImportRepository.BulkImportExamPortalCloud(linker.BatchIdGuidId);

                if (insertFromTempStorage)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public override Task<ActionResult<StudentTestDTO>> Put(int id, StudentTest entity)
        {
            throw new NotImplementedException();
        }

        [AllowAnonymous]
        [DisableRequestSizeLimit]
        [HttpPost("get-batch-guidid")]
        public async Task<ActionResult> GetBatchID()
        {
            try
            {
                var batchIDResult = await _bulkImportRepository.GetBatchID();
                return Ok(new { batchId = batchIDResult });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [DisableRequestSizeLimit]
        [HttpPost("get-template-url")]
        public async Task<ActionResult> getTemplateUrl(BulkImportTemplateLinker linker)
        {
            var filePath = "";
            try
            {
                //var batchIDResult = await _bulkImportRepository.GetBatchID();
                var uploads = Path.Combine(_env.WebRootPath, "Templates");
                if (linker.BulkImportTemplateUrl.Contains("Student"))
                {
                    filePath = Path.Combine(uploads, "StudentsBulkImport.xlsx");
                }
                else
                {
                    filePath = Path.Combine(uploads, "SubjectSectorsBulkImport.xlsx");
                }

                ///byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                await Task.Run(() =>
                {

                });
                return Ok(new { FilePath = filePath });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}

