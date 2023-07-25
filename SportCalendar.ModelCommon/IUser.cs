using System;

namespace SportCalendar.ModelCommon
{
    public interface IUser
    {
        Guid Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        Guid RoleId { get; set; }
        bool IsActive { get; set; }
        Guid UpdatedByUserId { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        string Username { get; set; }
    }
}
