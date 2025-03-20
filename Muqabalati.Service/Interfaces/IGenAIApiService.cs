using Muqabalati.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Service.Interfaces
{
    public interface IGenAIApiService : IDisposable
    {
        Task<string> GenerateContent(string apiKey, string prompt);
        Task<string> GetTheToneAsync(
                    string apiKey,
                    string tone,
                    string interviewLanguage);
        Task<string> GenerateIntroText(
                    string apiKey,
                    string applicantName,
                    string interviewerName,
                    string tone,
                    string topic,
                    string level,
                    string interviewLanguage);
        Task<string> GenerateQuestionText(
                    string apiKey,
                    int questionNum,
                    string topic,
                    string level,
                    string tone,
                    string terminologyLanguage,
                    string interviewLanguage);
        Task<string> GenerateConclusionText(
                    string apiKey,
                    string applicantName,
                    string tone,
                    string interviewLanguage);

        Task<InterviewReportDto> GenerateReportAsync(
            string apiKey,
            List<AnswerModel> answers,
            string[] questions);
    }
}
