using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Chinook.Data.Entities.Security;
using ChinookSystem.DAL.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
#endregion

namespace ChinookSystem.BLL.Security
{
    [DataObject]
    public class RoleManager:RoleManager<IdentityRole>
    {
        public RoleManager() : base(new RoleStore<IdentityRole>(new ApplicationDbContext()))
        {

        }
        public void AddDefaultRoles()
        {
            foreach (string roleName in SecurityRoles.DefaultSecurityRoles)
            {
                // Check if it exists
                if (!Roles.Any(r => r.Name == roleName))
                {
                    this.Create(new IdentityRole(roleName));
                }
            }
        }
        #region UserRole Administration
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<string> ListAllRoleNames()
        {
            return this.Roles.Where(r => r.Name != SecurityRoles.RegisteredUsers).Select(r => r.Name).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<RoleProfile> ListAllRoles()
        {
            var um = new UserManager();
            var results = from role in Roles.ToList()
                          select new RoleProfile
                          {
                              RoleId = role.Id,
                              RoleName = role.Name,
                              UserNames = role.Users.Select(r => um.FindById(r.UserId).UserName)
                          };
            return results.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddRole(RoleProfile role)
        {
            if (!this.RoleExists(role.RoleName))
            {
                this.Create(new IdentityRole(role.RoleName));
            }
            else
            {
                throw new Exception("Creation failed. " + role.RoleName + " already exists.");
            }
        }

        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public void DeleteRole(RoleProfile role)
        {
            var existing = this.FindById(role.RoleId);
            if (existing.Users.Count() == 0)
            {
                this.Delete(this.FindById(role.RoleId));
            }
            else
            {
                throw new Exception("Delete failed. " + role.RoleName + " has existing users. Reassign users first.");
            }


        }
        #endregion
    }

}
