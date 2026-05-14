using System;

namespace Gearbox.Application.DTOs
{
    public class StaffDto
    {
     
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string Role { get; set; }
      
    }
}
