using System.ComponentModel.DataAnnotations;

namespace Muqabalati.Service.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }
        public decimal Salary { get; set; }
    }
}
