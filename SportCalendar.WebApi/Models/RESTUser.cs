using System;

namespace SportCalendar.WebApi.Models
{
    public class RESTUser
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
    }
}