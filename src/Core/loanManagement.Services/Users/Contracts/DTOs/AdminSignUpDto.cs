using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loanManagement.Services.Users.Contracts.DTOs
{
    public class AdminSignUpDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string NationalId { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
