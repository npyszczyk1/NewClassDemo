using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.Data.Entities.Security
{
    public class RoleProfile
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public IEnumerable<string> UserNames { get; set; }
    }
}
