using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Muqabalati.Domain.Identity;
using Muqabalati.Service.DTOs.Admin;

namespace Muqabalati.Service.Mapper
{
    public class UserDTOsMapper
    {
        public static async Task<UserDto> ToUserDto(ApplicationUser user, Task<IList<string>> roles)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Phone = user.PhoneNumber,
                Password = user.PasswordHash,
                ImagePath = user.ImagePath,
                Role =  roles.GetAwaiter().GetResult().ToList(),
                IsLocked = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow
            };
        }

        public static ApplicationUser ToApplicationUser(UserDto userDto)
        {
            return new ApplicationUser
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserName = userDto.UserName,
                PhoneNumber = userDto.Phone,
                PasswordHash = userDto.Password,
                ImagePath = userDto.ImagePath
            };
        }
    }
}
