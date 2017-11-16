using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.Data.Entities.Security
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmation{ get; set; }
        public string RequestedPassord { get; set; }
        public IEnumerable<string> RoleMemberships { get; set; }
    }
}
