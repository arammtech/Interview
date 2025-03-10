using System.ComponentModel.DataAnnotations;

namespace Interview.Service.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;
    }
}
