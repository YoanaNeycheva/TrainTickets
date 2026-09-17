using Microsoft.AspNetCore.Identity;
using TrainTickets.Enums;

namespace TrainTickets.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public Role Role { get; set; }
    }
}
