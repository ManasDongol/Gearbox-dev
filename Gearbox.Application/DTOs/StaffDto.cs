using System;

namespace Gearbox.Application.DTOs
{
    public class StaffDto
    {
     
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
      
    }
}
