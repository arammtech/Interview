using System.ComponentModel.DataAnnotations;

namespace Muqabalati.Service.DTOs
{
    public class InterviewGeneratorDto
    {

        List<string> Topics { get; set; } = new List<string>
        {
            "","","","","","","","","",""
        };

        [Required]
        InterviewRequestDto request { get; set; }
    }
}
