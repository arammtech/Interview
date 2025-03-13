using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> StartInterview([FromBody] InterviewRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data", errors = ModelState });
            }

            try
            {
                //InterviewRequestDto request = new();
                //request.ApplicantName = "جون";
                //request.InterviewerName = "محمد";
                //request.Topic = "Backend c#";
                //request.Department = "Programming";
                //request.Level = "Jenior";
                //request.Tone = "السورية";
                //request.Language = "الانجليزية";
                //request.QuestionCount = 3;

                // Generate the interview session data
                var session = await _interviewService.GenerateInterviewSessionAsync(request);

                // Store the session JSON in HttpContext.Session
                HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(session));

                // Return the session data as JSON for client-side use
                return Ok(new { success = true, data = session });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while starting the interview.", details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the interview session data from session.
        /// </summary>
        [HttpGet("session")]
        public IActionResult GetInterviewSession()
        {
            try
            {
                // Retrieve the session JSON
                var sessionJson = HttpContext.Session.GetString(SessionKey);

                if (string.IsNullOrEmpty(sessionJson))
                {
                    return NotFound(new { message = "Interview session not found." });
                }

                // Deserialize and return the session data
                var session = JsonConvert.DeserializeObject<InterviewSessionDto>(sessionJson);
                return Ok(session);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the interview session.", details = ex.Message });
            }
        }
    }
    }
