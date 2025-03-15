using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muqabalati.Service.DTOs;
using Muqabalati.Service.Interfaces;
using Newtonsoft.Json;
using static Muqabalati.Utilities.Global.GlobalSettings;
using static Muqabalati.Service.GenAI.ParseFunctions;


namespace Muqabalati.Service.Implementations
{
    public class InterviewService : IInterviewService
    {
        private readonly IGenAIApiService _genAIApiService;

        public InterviewService(IGenAIApiService genAIApiService)
        {
            _genAIApiService = genAIApiService ?? throw new ArgumentNullException(nameof(genAIApiService));
        }

        // Generate a full interview session
        public async Task<InterviewSessionDto> GenerateInterviewSessionAsync(InterviewRequestDto request)
        {
            // Validate input request
            ValidateRequest(request);

            try
            {
                // Launch all tasks concurrently
                var introTask = _genAIApiService.GenerateIntroText(
                    apiKey,
                    request.ApplicantName,
                    request.InterviewerName,
                    request.Tone,
                    request.Topic,
                    request.SkillLevel,
                    request.Department,
                    request.InterviewLanguage
                );

                var questionsTask = _genAIApiService.GenerateQuestionText(
                    apiKey,
                    request.QuestionCount,
                    request.Topic,
                    request.SkillLevel,
                    request.Department,
                    request.Tone,
                    request.TerminologyLanguage,
                    request.InterviewLanguage
                );

                var conclusionTask = _genAIApiService.GenerateConclusionText(
                    apiKey,
                    request.ApplicantName,
                    request.Tone,
                    request.InterviewLanguage
                );

                // Await all tasks concurrently
                await Task.WhenAll(introTask, questionsTask, conclusionTask);

                // Parse questions (dependent on questionsTask result)
                var questions = await ParseQuestions(await questionsTask);

                // Deserialize the intro text response
                var introtextResponse = JsonConvert.DeserializeObject<GenAIApiResponse>(await introTask);
                if (introtextResponse == null || introtextResponse.Candidates == null || !introtextResponse.Candidates.Any())
                {
                    throw new InvalidOperationException("Failed to deserialize the intro text response or it's empty.");
                }

                // Extract intro text
                var introText = introtextResponse.Candidates.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                                ?? throw new InvalidOperationException("Intro text is missing in the deserialized response.");

                // Deserialize the conclusion text response
                var conclusiontextResponse = JsonConvert.DeserializeObject<GenAIApiResponse>(await conclusionTask);
                if (conclusiontextResponse == null || conclusiontextResponse.Candidates == null || !conclusiontextResponse.Candidates.Any())
                {
                    throw new InvalidOperationException("Failed to deserialize the conclusion text response or it's empty.");
                }

                // Extract conclusion text
                var conclusionText = conclusiontextResponse.Candidates.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                                     ?? throw new InvalidOperationException("Conclusion text is missing in the deserialized response.");

                // Create and return the interview session DTO
                return new InterviewSessionDto
                {
                    ApplicantName = request.ApplicantName,
                    IntroText = introText,
                    Questions = questions,
                    Tone = request.Tone,
                    ConclusionText = conclusionText
                };

            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging framework)
                Console.Error.WriteLine($"Error generating interview session: {ex.Message}");
                throw new InvalidOperationException("An error occurred while generating the interview session.", ex);
            }
        }

        // Validate the interview request object
        private void ValidateRequest(InterviewRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            if (string.IsNullOrEmpty(request.ApplicantName))
                throw new ArgumentException("Applicant name cannot be null or empty.", nameof(request.ApplicantName));

            if (string.IsNullOrEmpty(request.InterviewerName))
                throw new ArgumentException("Interviewer name cannot be null or empty.", nameof(request.InterviewerName));

            if (request.QuestionCount <= 0)
                throw new ArgumentException("Question count must be greater than zero.", nameof(request.QuestionCount));
        }


        public async Task<InterviewReportDto> GenerateInterviewReport( List<AnswerModel> answers, string[] questions)
        {
            try
            {
                // إنشاء التقرير بناءً على الإجابات والأسئلة
                var report = await _genAIApiService.GenerateReportAsync(apiKey, answers, questions);

                return report;
            }
            catch (Exception ex)
            {
                // تسجيل الخطأ والتعامل معه
                Console.WriteLine($"Error generating interview report: {ex.Message}");
                throw; // أو إرجاع تقرير فارغ أو مع رسالة خطأ
            }
        }

    }
}
