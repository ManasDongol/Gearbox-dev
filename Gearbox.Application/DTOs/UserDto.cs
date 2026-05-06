using System;

namespace Gearbox.Application.DTOs
{
    public class UserDto
    {
       
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
   
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        

        public bool IsActive { get; set; }
        public bool emailVerified { get; set; }
    }
}
