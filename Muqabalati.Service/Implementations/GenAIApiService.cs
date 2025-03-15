using System.Text;
using Muqabalati.Service.DTOs;
using Muqabalati.Service.Interfaces;
using Newtonsoft.Json;
using static Muqabalati.Service.GenAI.GeneratePrompts;
using static Muqabalati.Service.GenAI.ParseFunctions;

namespace Muqabalati.Service.Implementations
{
    public class GenAIApiService : IGenAIApiService
    {
        private readonly HttpClient _httpClient;

        public GenAIApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<InterviewReportDto> GenerateReportAsync(
            string apiKey,
            List<AnswerModel> answers,
            string[] questions)
        {
            // إنشاء النص الموجه (Prompt) بناءً على الأسئلة والإجابات.
            string prompt = GenerateReportPrompt(answers, questions);

            // إرسال النص الموجه إلى Gemini AI API للحصول على تحليل النص.
            string jsonResponse = await GenerateContent(apiKey, prompt);

            // تحليل الاستجابة JSON إلى كائن InterviewReportDto.
            var report = await ParseInterviewReport(jsonResponse);

            return report;
        }




        public async Task<string> GenerateIntroText(
      string apiKey,
      string applicantName,
      string interviewerName,
      string tone,
      string topic,
      string level,
      string department,
      string interviewLanguage)
        {
            var introPrompt = GenerateIntroPrompt(applicantName, interviewerName, tone, topic, level, department, interviewLanguage);
            return await GenerateContent(apiKey, introPrompt);
        }

        public async Task<string> GenerateConclusionText(
            string apiKey,
            string applicantName,
            string tone,
            string interviewLanguage)
        {
            var conclusionPrompt = GenerateConclusionPrompt(applicantName, tone, interviewLanguage);
            return await GenerateContent(apiKey, conclusionPrompt);
        }


        public async Task<string> GenerateQuestionText(
            string apiKey,
            int questionNum,
            string topic,
            string level,
            string department,
            string tone,
            string terminologyLanguage,
            string interviewLanguage)
        {
            var questionPrompt = GenerateQuestionPrompt(questionNum, topic, level, department, tone,terminologyLanguage, interviewLanguage);
            return await GenerateContent(apiKey, questionPrompt);
        }

        // Method to send a prompt to the GenAIApi API and get the response
        public async Task<string> GenerateContent(string apiKey, string prompt)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            // Request body for the API
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            // Serialize request body to JSON
            string json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // Send POST request to the API
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // Ensure success code; throw exception if not
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody; // Return the API response
            }
            catch (HttpRequestException ex)
            {
                // Log or handle HTTP-specific errors
                Console.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
            catch (System.Text.Json.JsonException ex)
            {
                // Log or handle JSON serialization/deserialization errors
                Console.WriteLine($"JSON Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Handle generic exceptions
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }
        }





        // Dispose HttpClient properly
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
