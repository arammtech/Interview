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
                var interviewSessionDto = await _interviewService.GenerateInterviewSessionAsync(request);

                // Store the session JSON in HttpContext.Session
                HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(interviewSessionDto));
                string sessionDate = HttpContext.Session.GetString(SessionKey);
                Console.WriteLine(sessionDate);

                return Ok(new { success = true, data = interviewSessionDto });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while starting the interview.", details = ex.Message });
            }
        }

        [HttpPost("rate")]
        public async Task<IActionResult> RateInterview([FromBody] string[] questions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data", errors = ModelState });
            }

            try
            {
                // Get the interviewSession form the session
                // Plus the question 
                // var session

                //var rate = await _interviewService.GetRateAsync(session);


                return Ok(new { success = true});
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while starting the interview.", details = ex.Message });
            }
        }
    }
    }
