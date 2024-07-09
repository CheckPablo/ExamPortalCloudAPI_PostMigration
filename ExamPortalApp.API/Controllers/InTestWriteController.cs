using AutoMapper;
using ExamPortalApp.Contracts.Data.Dtos;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Speech.Synthesis;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using Newtonsoft.Json;
using ExamPortalApp.Infrastructure.Extensions;
using System.Diagnostics;
using System.Web;
namespace ExamPortalApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InTestWriteController(IInTestWriteRepository inTestWriteRepository, IMapper mapper, IHttpContextAccessor contextAccessor, IWebHostEnvironment env, IConfiguration configuration) : CrudControllerBase<StudentTestDTO, StudentTest>(mapper)
    {
        private readonly IWebHostEnvironment _env = env;
        private readonly IInTestWriteRepository _inTestWriteRepository = inTestWriteRepository;
        private readonly List<object> installedVoiceList = [];
        readonly List<InstalledVoice> installedVoices = [];
        private string[] fileNames;

        private readonly IConfiguration _configuration = configuration;

        //private IFormFileCollection scannedFiles;
        private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

        [HttpDelete("{id}")]
        public override async Task<ActionResult<StudentTestDTO>> Delete(int id)
        {
            try
            {
                var response = await _inTestWriteRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("launch-lockscreen-application")]
        public async Task<IActionResult> LaunchLockScreenApplication()
        {
            RunExecutable("test2");
            string contentRootPath = _env.ContentRootPath;
            string webRootPath = _env.WebRootPath;
            return Content(contentRootPath + "\n" + webRootPath);
        }
        private void RunExecutable(string parameter)
        {
            try
            {
                string contentRootpath = _env.ContentRootPath;
                string webrootPath = _env.WebRootPath;
                var fileName = contentRootpath + webrootPath + @"\content\LockDownBrowserOEM.exe";

                Process p = new();
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = fileName;
                p.StartInfo.Arguments = parameter;
                p.StartInfo.WorkingDirectory = contentRootpath + webrootPath + @"\content\";

                p.Start();
                p.WaitForExit();

            }
            catch (Exception)
            {

                throw;
            }


        }
        [HttpGet]
        public override async Task<ActionResult<IEnumerable<StudentTestDTO>>> Get()
        {
            try
            {
                var testSecurityLevels = await _inTestWriteRepository.GetAllAsync();
                var result = _mapper.Map<IEnumerable<StudentTestDTO>>(testSecurityLevels);

                return Ok(testSecurityLevels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("installedVoices")]
        public List<object> InstalledVoices(int id)
        {
            // Initialize a new instance of the SpeechSynthesizer.  
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                // var windowsVoices = synth.GetInstalledVoices().ToList();
                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    VoiceInfo? info = voice?.VoiceInfo;
                    //synth.SelectVoice(voice.VoiceInfo.Name);
                    var voiceEntry = new { Name = info.Name, lang = info.Culture.Name, };
                    installedVoiceList.Add(voiceEntry);
                    installedVoices.Add(voice);
                }
                //string[] str = installedVoiceList.ToArray();
                //var windowsVoices = installedVoiceList;
                return installedVoiceList;
                //Console.WriteLine(installedVoiceList); 
            }

        }

        /*[HttpGet("installedVoices")]
        public List<string> InstalledVoices(int id)
        {
            // Initialize a new instance of the SpeechSynthesizer.  
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {

                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    // VoiceInfo info = voice.VoiceInfo;
                    string fir = synth.SelectVoice(voice.VoiceInfo.Name).ToString();
                    installedVoiceList.Add(synth.SelectVoice(voice.VoiceInfo.Name));
                }
                //string[] str = installedVoiceList.ToArray();
                return installedVoiceList;
            }

        }*/

        [HttpGet("{id}")]
        public override async Task<ActionResult<StudentTestDTO>> Get(int id)
        {
            try
            {
                var testSecurityLevel = await _inTestWriteRepository.GetAsync(id);
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
                var response = await _inTestWriteRepository.AddAsync(StudentTest);
                var result = _mapper.Map<StudentTestDTO>(response);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        [HttpPost("upload-answer-document")]
        public async Task<ActionResult<StudentTestAnswers>> UploadAnswerDocumentAsync()
        {
            // System.Web.HttpPostedFile data = HttpContext.Current.Request.Files[0];
            try
            {
                var data = Request.Form["data"].ToString();
                var form = JsonConvert.DeserializeObject<StudentTestSave>(data);
                //if (Request.Form.Files.Count() > 0)
                if (form is not null)
                {
                    var file = Request.Form.Files[0];
                    var response = await _inTestWriteRepository.UploadStudentAnswerDocumentAsync(form.TestId, form.StudentId, form.Accomodation ?? false,
                        form.Offline ?? false, form.FullScreenClosed ?? false, form.KeyPress ?? false, form.LeftExamArea ?? false, form.TimeRemaining, form.AnswerText, form.fileName, file);
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Data or file not provided");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("save-irregular-keypressevent")]
        public async Task<ActionResult<KeyPressTracking>> SaveIrregularKeyPress(InvalidKeyPressEntries invalidKeyPressEntries)
        {
            if(invalidKeyPressEntries.Reason == null) {

                if(invalidKeyPressEntries.Event.Contains("windows")){
                invalidKeyPressEntries.Reason = invalidKeyPressEntries.Event; 
                }
                
                else if(invalidKeyPressEntries.Event.Contains("Ctrl + Alt + Delete")){
                invalidKeyPressEntries.Reason = invalidKeyPressEntries.Event; 
                }

                else if(invalidKeyPressEntries.Event.Contains("handleKeyboardEvent")){
                invalidKeyPressEntries.Reason = invalidKeyPressEntries.Event;
                }

                else if(invalidKeyPressEntries.Event.Contains("LEFT EXAM AREA")){
                invalidKeyPressEntries.Reason = invalidKeyPressEntries.Event; 
                }
                
                else{
                    invalidKeyPressEntries.Reason = "Invalid key combination"; 
                }
            }


            try
            {
                var result = await _inTestWriteRepository.SaveIrregularKeyPress(invalidKeyPressEntries);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[EnableCors("MyAllowSpecificOrigins")]
        [HttpPost("verify-scanned-imagesotp")]
        public async Task<ActionResult<List<string>>> VerifyScannedImagesOTP(ScannedImagesOTP scannedImagesOTP)
        {
            try
            {
                var result = await _inTestWriteRepository.VerifyImagesOTP(scannedImagesOTP);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*[HttpPost("verify-scanned-imagesotp")]
       public async Task<ActionResult> VerifyScannedImagesOTP(ScannedImagesOTP scannedImagesOTP)
       {
           //var path = $@"d:\smth\upload\{id}.jpeg";
           var result = (List<string>)await _inTestWriteRepository.VerifyImagesOTP(scannedImagesOTP);
           foreach(var c in result)
           {
               scannedImagesOTP.URL = ""; 
           }
           //byte[] bytes = System.IO.File.ReadAllBytes(result);
           return  PhysicalFile(scannedImagesOTP.URL, "image/jpeg");
       }
      [HttpGet]
       public ActionResult Get(string id)
       {
                  //byte[] bytes = System.IO.File.ReadAllBytes(scannedImagesOTP.URL);
                //string base64String = Convert.ToBase64String(bytes);
                //return Content("data:application/image;jpeg," + base64String);
           var path = $@"d:\smth\upload\{id}.jpeg";
           byte[] bytes = System.IO.File.ReadAllBytes(path);
           return File(bytes, "image/jpeg");
       }*/


        [HttpPost("scandocument")]
        public async Task<ActionResult<string>> ScanDocument(QrCodeModel qrcodeModel)
        {
            try
            {
                string testId = qrcodeModel.testId;
                string studentId = qrcodeModel.studentId;
                var QrCodeEntry = testId + "" + studentId;
                //var chh = _contextAccessor.HttpContext?.Request.BaseUrl();
                var chh = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                //string baseUrl = string.Format("{ 0}://{1}{2}", Request.Scheme, Request.Host, Request.PathBase.Value.ToString());
                var badseUrl = Request.GetTypedHeaders().Referer.ToString() ?? "";
                return Ok(new { badseUrl });

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [AllowAnonymous]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        [HttpPost("add-scannedimages")]
        public async Task<ActionResult> UploadFiles(List<IFormFile> files)
        {
            try
            {
                var x = Request.Form.Files[0].OpenReadStream();
                var testId = "";
                var studentId = "";
                var data = (Request.Form["data"]).ToString();
                if (data.Length > 0)
                {
                    //{ "testId":4383,"studentId":131231}
                    char[] delimiterChars = { ' ', ',', '.', ':', '\t', '}' };
                    string[] studentTestData = data.Split(delimiterChars);
                    testId = studentTestData[1];
                    studentId = studentTestData[3];
                }
                /*testId = testId.ToString().Split(":");
                var form = JsonConvert.DeserializeObject<Test>(data);*/
                long size = files.Sum(f => f.Length);
                var scannedFiles = Request.Form.Files;
                if (scannedFiles.Count == 0)
                {
                    return BadRequest("No files uploaded");
                }

                fileNames = scannedFiles.Select(x => x.FileName).ToArray();
                await UploadExtensions.WriteFile(scannedFiles, testId);

                var scanResultOTP = await _inTestWriteRepository.UploadScannedImagetoDB(fileNames, testId, studentId);

                return Ok(new { count = files.Count, size, otp = scanResultOTP });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("windowstts/{selectedVoice}/{selectedText}")]
        //public async Task<ActionResult> WindowsTTS(string selectedVoice, string selectedText)
        [HttpPost("windowstts")]
        public async Task<ActionResult> WindowsTTS(WindowsSpeechModel? winspeech)
        {
            //SpVoice voice = new SpVoice();

            try
            {
                //SpVoice voice = new SpVoice();
                using (SpeechSynthesizer synth = new SpeechSynthesizer { Volume = 50, Rate = 0 })
                {

                    synth.SelectVoice(winspeech?.selectedVoice);
                    synth.Speak(winspeech?.selectedText);
                    //grpAdjustments.Enabled = false;
                    //synth.Speak(txtTextToSpeak.Text);
                    //grpAdjustments.Enabled = true;

                    //Console.WriteLine(installedVoices);
                }

                // var tts = await _inTestWriteRepository.ConvertWindowsTTS(winspeech);
                // var result = _mapper.Map<UserDto>(tts);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public override Task<ActionResult<StudentTestDTO>> Put(int id, StudentTest entity)
        {
            throw new NotImplementedException();
        }


        [AllowAnonymous]
        [HttpGet("get-student-sebsettings/{uniqueName}/{testId}/{studentUserId}/{testName}/{domain}")]
        public ActionResult GetSEBWindowsSettings(string uniqueName, string testId, string studentUserId, string testName, string domain)
        {

            try
            {
                string contentRootpath = _env.ContentRootPath;
                string webrootPath = _env.WebRootPath;
                var fileName = contentRootpath +  $@"/templates/SebExtensionWins.xml";
                string xml = System.IO.File.ReadAllText(fileName);
                string studentLogOn = $"<string>https://{domain}/portal/test-writing/test-writing-management/{uniqueName}/{testId}/{studentUserId}/{testName}</string>";
                //string studentLogOn = $"<string>https://{domain}/portal/test-writing/test-writing-management/{uniqueName}/{testId}/{studentUserId}/{testName}</string>";
                string cleanStudentLogOn = studentLogOn.Replace(" ","%20");
                xml = xml.Replace("[[studentLogOn]]", cleanStudentLogOn);
                Response.Headers.Add("Content-Disposition", "attachment; filename=\"LV17Ra4PCbMSJpLM90md.seb\"");
                return Content(xml, "html/text");
            }
            catch (Exception ex)
            {
                string contentRootpath = _env.ContentRootPath;
                string webrootPath = _env.WebRootPath;
                //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var docPath = contentRootpath + webrootPath + @"/templates/";

                //Write the string array to a new file named "WriteLines.txt".
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
                {

                    outputFile.WriteLine(ex.Message.ToString());
                }
                return BadRequest(ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpGet("get-student-seb-mac-settings/{uniqueName}/{testId}/{studentUserId}/{testName}/{domain}")]
        public ActionResult GetSEBMacSettings(string uniqueName, string testId, string studentUserId, string testName, string domain)
        {

            try
            {
                string contentRootpath = _env.ContentRootPath;
                string webrootPath = _env.WebRootPath;

                var fileName = contentRootpath + $@"/templates/SebExtensionMac.xml";
                string xml = System.IO.File.ReadAllText(fileName);

                var queryString = HttpUtility.ParseQueryString("?param1=value");
                    
                // Adding a parameter
                queryString.Add("", "my value");

                // Url Encoding the whole thing
                queryString.ToString();
                string studentLogOn = $"<string>https://{domain}/portal/test-writing/test-writing-management/{uniqueName}/{testId}/{studentUserId}/{testName}</string>";
                //string studentLogOn = $"<string>http://{domain}/portal/test-writing/test-writing-management/{uniqueName}/{testId}/{studentUserId}/{testName}</string>";

                string cleanStudentLogOn = studentLogOn.Replace(" ","%20");
                xml = xml.Replace("[[studentLogOn]]", cleanStudentLogOn);

                Response.Headers.Add("Content-Disposition", "attachment; filename=\"LV17Ra4PCbMSJpLM90md.seb\"");
                return Content(xml, "html/text");
            }
            catch (Exception ex)
            {
                string contentRootpath = _env.ContentRootPath;
                string webrootPath = _env.WebRootPath;
                //string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var docPath = contentRootpath + webrootPath + @"/templates/";

                //Write the string array to a new file named "WriteLines.txt".
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
                {

                    outputFile.WriteLine(ex.Message.ToString());
                }
                return BadRequest(ex.Message);
            }
        }
    }
}

