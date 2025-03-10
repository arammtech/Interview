namespace Interview.Service.DTOs.Admin
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? ImagePath { get; set; }
        public List<string> Role { get; set; }
        public bool IsLocked { get; set; }
    }
}
