using System.ComponentModel.DataAnnotations;
using Muqabalati.Domain.Identity;

namespace Muqabalati.Web.Areas.Admin.ViewModels
{
    public class ChangeUserRoleDto
    {
        public int Id { get; set; }
        public string oldRole { get; set; }
        public string? newRole { get; set; }
        public List<ApplicationRole> Roles { get; set; } = [];
    }
}
