using System.Collections.Generic;
using System.Threading.Tasks;
using Muqabalati.Service.DTOs;
using Muqabalati.Service.Interfaces;
using static Muqabalati.Utilities.Global.GlobalSettings;
namespace Muqabalati.Service.Implementations
{
    public class InterviewService : IInterviewService
    {
        private readonly IGenAIApiService _GenAIApiService;

        public InterviewService(IGenAIApiService GenAIApiService)
        {
            _GenAIApiService = GenAIApiService;
        }

        // Generate a full interview session
        public async Task<InterviewSessionDto> GenerateInterviewSessionAsync(InterviewRequestDto request)
        {
            // Generate all the parts of the interview
            var introText = await _GenAIApiService.GenerateIntroText(
                apiKey,
                request.ApplicantName,
                request.InterviewerName,
                request.Tone,
                request.Topic,
                request.SkillLevel,
                request.Department
            );

            var questionsResponse = await _GenAIApiService.GenerateQuestionText(
                apiKey,
                request.QuestionCount,
                request.Topic,
                request.SkillLevel,
                request.Department,
                request.Tone,
                request.TerminologyLanguage
            );

            var questions = await _GenAIApiService.ParseQuestions(questionsResponse);

            var conclusionText = await _GenAIApiService.GenerateConclusionText(apiKey,request.ApplicantName,request.Tone);

            // Create the session object
            return new InterviewSessionDto
            {
                ApplicantName = request.ApplicantName,
                IntroText = introText,
                Questions = questions,
                ConclusionText = conclusionText
            };
        }
    }
}
