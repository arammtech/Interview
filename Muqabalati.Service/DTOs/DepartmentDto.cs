using System.ComponentModel.DataAnnotations;

namespace Muqabalati.Service.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;
    }
}
