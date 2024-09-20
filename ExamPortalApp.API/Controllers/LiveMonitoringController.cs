using AutoMapper;
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
    public class LiveMonitoringController(ILiveMonitoringRepository liveMonitoring, IMapper mapper) : Controller
    {
        private readonly ILiveMonitoringRepository _liveMonitoring = liveMonitoring;
        private readonly IMapper _mapper = mapper;

        [HttpPost("getLiveMonitoringCanidateList")]
        public async Task<IActionResult> GetLiveMonitoringCanidateListAsync(int testId, int candidateSearchType, string? name)
        {
            try
            {
                var liveMonitoringCanidateList = await _liveMonitoring.GetLiveMonitoringCanidateList(testId, candidateSearchType, name);
                var result = _mapper.Map<IEnumerable<LiveMonitoring>>(liveMonitoringCanidateList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetLiveMonitoringIrregularities")]
        public async Task<IActionResult> GetLiveMonitoringIrregularities(int testId, int studentId)
        {
            try
            {
                var keyPressTrackings = await _liveMonitoring.GetLiveMonitoringIrregularities(testId, studentId);
                var result = _mapper.Map<IEnumerable<KeyPressTracking>>(keyPressTrackings);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetStudentTestLogs")]
        public async Task<IActionResult> GetStudentTestLogs(int testId, int studentId)
        {
            try
            {
                var studentTestLogs = await _liveMonitoring.GetStudentTestLogs(testId, studentId);
                var result = _mapper.Map<IEnumerable<StudentTestLog>>(studentTestLogs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

         [HttpPost("GetStudentOfflineTestLogs")] 
        public async Task<IActionResult> GetStudentOfflineTestLogs(int testId, int studentId)
        {
            try
            {
                var studentOfflineTestLogs = await _liveMonitoring.GetStudentOfflineTestLogs(testId, studentId);
                var result = _mapper.Map<IEnumerable<KeyPressTracking>>(studentOfflineTestLogs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost("GetIrregularityTestLogs")]
        public async Task<IActionResult> GetIrregularityTestLogs(int testId, int studentId)
        {
            try
            {
                var studentTestLogs = await _liveMonitoring.GetIrregularityTestLogs(testId, studentId);
                var result = _mapper.Map<IEnumerable<StudentTestLog>>(studentTestLogs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("GetInvalidKeyPresses")]
        public async Task<IActionResult> GetInvalidKeyPresses(int testId, int studentId)
        {
            try
            {
                var keyPressTrackings = await _liveMonitoring.GetInvalidKeyPresses(testId, studentId);
                var result = _mapper.Map<IEnumerable<KeyPressTracking>>(keyPressTrackings);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetLiveMonitoringStudentAnswerProgress")]
        public async Task<IActionResult> GetLiveMonitoringStudentAnswerProgress(int testId, int studentId)
        {
            try
            {
                var answerProgressTrackings = await _liveMonitoring.GetLiveMonitoringStudentAnswerProgress(testId, studentId);
                var result = _mapper.Map<IEnumerable<AnswerProgressTracking>>(answerProgressTrackings);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-extraTime")]
        public async Task<ActionResult> LinkStudents(StudentTestExtraTimeLinker linker)
        {
            try
            {
                var result = await _liveMonitoring.LinkStudentsExtraTimeAsync(linker);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("end-test")]
        public async Task<ActionResult> EndTest(EndTestLinker linker)
        {
           try{
            var result = await _liveMonitoring.EndTestAsync(linker);
            return Ok(result);
           }
           catch (Exception ex){
            return BadRequest(ex.Message);
           }
        }
    }
}