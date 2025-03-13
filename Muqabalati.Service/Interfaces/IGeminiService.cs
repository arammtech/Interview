using Muqabalati.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.Interfaces
{
    public interface IGeminiService : IDisposable
    {
        Task<string> GenerateContent(string apiKey, string prompt);

        Task<string> GenerateIntroText(
                    string apiKey,
                    string applicantName,
                    string interviewerName,
                    string tone,
                    string topic,
                    string level,
                    string department);
        Task<string> GenerateQuestionText(string apiKey, int questionNum, string topic, string level, string department, string tone, string language);
        Task<string> GenerateConclusionText(string apiKey, string applicantName, string tone);
        Task<List<QuestionModel>> ParseQuestions(string jsonResponse);
    }
}
