using System.Threading.Tasks;
using Muqabalati.Service.DTOs;

namespace Muqabalati.Service.Interfaces
{
    public interface IInterviewService
    {
        Task<InterviewSessionDto> GenerateInterviewSessionAsync(InterviewRequestDto request);
    }
}
