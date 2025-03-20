using Microsoft.AspNetCore.Mvc;
using Muqabalati.Service.DTOs;
using Muqabalati.Service.Interfaces;
using Newtonsoft.Json;

namespace Muqabalati.Web.Areas.Customer.APIControllers
{
    [ApiController]
    [Route("api/Customer/[controller]")]
    public class InterviewController : Controller
    {
        private readonly IInterviewService _interviewService;
        private const string SessionKey = "InterviewSession";

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        /// <summary>
        /// Starts the interview session by generating data and saving it to session.
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> StartInterview()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data", errors = ModelState });
            } 
            try  
            {
                string interviewSessionJson = HttpContext.Session.GetString(SessionKey);

                if (string.IsNullOrEmpty(interviewSessionJson))
                {
                    return BadRequest("Interview session not found.");
                }

                var interviewSessionDto = JsonConvert.DeserializeObject<InterviewSessionDto>(interviewSessionJson);
                return Ok(new { success = true, data = interviewSessionDto });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while starting the interview.", details = ex.Message });
            }
        }


        [HttpPost("result")]
        public async Task<IActionResult> Result([FromBody] List<AnswerModel> answers)
        {

            try
            {
                string interviewSessionJson = HttpContext.Session.GetString("InterviewSession");

                if (string.IsNullOrEmpty(interviewSessionJson))
                {
                    return BadRequest("Interview session not found.");
                }

                var session = JsonConvert.DeserializeObject<InterviewSessionDto>(interviewSessionJson);

                if (session == null || session.Questions == null)
                {
                    return BadRequest("Invalid interview session data.");
                }

                var questions = session.Questions.Select(q => q.OriginalQuestion).ToArray();

                var report = await _interviewService.GenerateInterviewReport(answers, questions);

                TempData["InterviewReport"] = JsonConvert.SerializeObject(report);

                return Ok(new { success = true });
            }
            catch
            {
                return View("Error");
            }
        }


        [HttpPost("end")]
        public async Task<IActionResult> EndInterview()
        {
            try
            {
                HttpContext.Session.SetString(SessionKey,"");

                return Ok(new { success = true, message = "Interview ended successfuly" });

            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while ending the interview.", details = ex.Message });
            }
        }

    }
    }
