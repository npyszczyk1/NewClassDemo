using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Chinook.Data.Entities.Security;
using Chinook.Data.Entities;
using Chinook.Data.POCOs;
using ChinookSystem.DAL;
using ChinookSystem.DAL.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
#endregion

namespace ChinookSystem.BLL.Security
{
    [DataObject]
    public class UserManager : UserManager<ApplicationUser>
    {
        #region Constants
        private const string STR_DEFAULT_PASSWORD = "Pa$$word1";
        /// <summary>Requires FirstName and LastName</summary>
        private const string STR_USERNAME_FORMAT = "{0}{1}";
        /// <summary>Requires UserName</summary>
        private const string STR_EMAIL_FORMAT = "{0}@Chinook.ca";
        private const string STR_WEBMASTER_USERNAME = "Webmaster";
        #endregion

        public UserManager()
            : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
        {
        }

        public void AddWebMaster()
        {
            //Users accesses all the records on the AspNetUsers table
            //UserName is the user logon user id (dwelch)
            if (!Users.Any(u => u.UserName.Equals(STR_WEBMASTER_USERNAME)))
            {
                //create a new instance that will be used as the data to
                //   add a new record to the AspNetUsers table
                //dynamically fill two attributes of the instance
                var webmasterAccount = new ApplicationUser()
                {
                    UserName = STR_WEBMASTER_USERNAME,
                    Email = string.Format(STR_EMAIL_FORMAT, STR_WEBMASTER_USERNAME)
                };

                //place the webmaster account on the AspNetUsers table
                this.Create(webmasterAccount, STR_DEFAULT_PASSWORD);

                //place an account role record on the AspNetUserRoles table
                //.Id comes from the webmasterAccount and is the pkey of the Users table
                //role will comes from the Entities.Security.SecurityRoles
                this.AddToRole(webmasterAccount.Id, SecurityRoles.WebsiteAdmins);
            }
        }

        public void AddEmployees()
        {
            using (var context = new ChinookContext())
            {
                //get all current employees
                //linq query will not execute as yet
                //return datatype will be IQueryable<EmployeeListPOCO>
                var currentEmployees = from x in context.Employees
                                       select new EmployeeListPOCO
                                       {
                                           EmployeeId = x.EmployeeId,
                                           FirstName = x.FirstName,
                                           LastName = x.LastName
                                       };

                //get all employees who have an user account
                //Users needs to be in memory therfore use .ToList()
                //POCO EmployeeID is an int
                //the Users Employee id is an int?
                //since we will only be retrieving
                //  Users that are employees (ID is not null)
                //  we need to convert the nullable int into
                //  a require int
                //the results of this query will be in memory
                var UserEmployees = from x in Users.ToList()
                                    where x.EmployeeID.HasValue
                                    select new RegisteredEmployeePOCO
                                    {
                                        UserName = x.UserName,
                                        EmployeeId = int.Parse(x.EmployeeID.ToString())
                                    };
                //loop to see if auto generation of new employee
                //Users record is needed
                //the foreach cause the delayed execution of the
                //linq above
                foreach (var employee in currentEmployees)
                {
                    //does the employee NOT have a logon (no User record)
                    if (!UserEmployees.Any(us => us.EmployeeId == employee.EmployeeId))
                    {
                        //create a suggested employee UserName
                        //firstname initial + LastName: dwelch
                        var newUserName = employee.FirstName.Substring(0, 1) + employee.LastName;

                        //create a new User ApplicationUser instance
                        var userAccount = new ApplicationUser()
                        {
                            UserName = newUserName,
                            Email = string.Format(STR_EMAIL_FORMAT, newUserName),
                            EmailConfirmed = true
                        };
                        userAccount.EmployeeID = employee.EmployeeId;
                        //create the Users record
                        IdentityResult result = this.Create(userAccount, STR_DEFAULT_PASSWORD);

                        //result hold the return value of the creation attempt
                        //if true, account was created,
                        //if false, an account already exists with that username
                        if (!result.Succeeded)
                        {
                            //name already in use
                            //get a UserName that is not in use
                            newUserName = VerifyNewUserName(newUserName);
                            userAccount.UserName = newUserName;
                            this.Create(userAccount, STR_DEFAULT_PASSWORD);
                        }

                        //create the staff role in UserRoles
                        this.AddToRole(userAccount.Id, SecurityRoles.Staff);
                    }
                }
            }
        }

        public string VerifyNewUserName(string suggestedUserName)
        {
            //get a list of all current usernames (customers and employees)
            //  that start with the suggestusername
            //list of strings
            //will be in memory
            var allUserNames = from x in Users.ToList()
                               where x.UserName.StartsWith(suggestedUserName)
                               orderby x.UserName
                               select x.UserName;
            //set up the verified unique UserName
            var verifiedUserName = suggestedUserName;

            //the following for() loop will continue to loop until
            // an unsed UserName has been generated
            //the condition searches all current UserNames for the
            //currently generated verified used name (inside loop code)
            //if found the loop will generate a new verified name
            //   based on the original suggest username and the counter
            //This loop continues until an unused username is found
            //OrdinalIgnoreCase : case does not matter
            for (int i = 1; allUserNames.Any(x => x.Equals(verifiedUserName,
                         StringComparison.OrdinalIgnoreCase)); i++)
            {
                verifiedUserName = suggestedUserName + i.ToString();
            }

            //return teh finalized new verified user name
            return verifiedUserName;
        }

        #region UserRole Adminstration
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<UserProfile> ListAllUsers()
        {
            var rm = new RoleManager();
            List<UserProfile> results = new List<UserProfile>();
            var tempresults = from person in Users.ToList()
                              select new UserProfile
                              {
                                  UserId = person.Id,
                                  UserName = person.UserName,
                                  Email = person.Email,
                                  EmailConfirmation = person.EmailConfirmed,
                                  EmployeeId = person.EmployeeID,
                                  CustomerId = person.CustomerID,
                                  RoleMemberships = person.Roles.Select(r => rm.FindById(r.RoleId).Name)
                              };
            //get any user first and last names
            using (var context = new ChinookContext())
            {
                Employee tempEmployee;
                foreach (var person in tempresults)
                {
                    if (person.EmployeeId.HasValue)
                    {
                        tempEmployee = context.Employees.Find(person.EmployeeId);
                        if (tempEmployee != null)
                        {
                            person.FirstName = tempEmployee.FirstName;
                            person.LastName = tempEmployee.LastName;
                        }
                    }
                    results.Add(person);
                }
            }
            return results.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Insert, false)]
        public void AddUser(UserProfile userinfo)
        {
            if (string.IsNullOrEmpty(userinfo.EmployeeId.ToString()))
            {
                throw new Exception("Employee ID is missing. Remember Employee must be on file to get an user account.");

            }
            else
            {
                EmployeeController sysmgr = new EmployeeController();
                Employee existing = sysmgr.Employee_Get(int.Parse(userinfo.EmployeeId.ToString()));
                if (existing == null)
                {
                    throw new Exception("Employee must be on file to get an user account.");
                }
                else
                {
                    var userAccount = new ApplicationUser()
                    {
                        EmployeeID = userinfo.EmployeeId,
                        CustomerID = userinfo.CustomerId,
                        UserName = userinfo.UserName,
                        Email = userinfo.Email
                    };
                    IdentityResult result = this.Create(userAccount,
                        string.IsNullOrEmpty(userinfo.RequestedPassord) ? STR_DEFAULT_PASSWORD
                        : userinfo.RequestedPassord);
                    if (!result.Succeeded)
                    {
                        //name was already in use
                        //get a UserName that is not already on the Users Table
                        //the method will suggest an alternate UserName
                        userAccount.UserName = VerifyNewUserName(userinfo.UserName);
                        this.Create(userAccount, STR_DEFAULT_PASSWORD);
                    }
                    foreach (var roleName in userinfo.RoleMemberships)
                    {
                        //this.AddToRole(userAccount.Id, roleName);
                        AddUserToRole(userAccount, roleName);
                    }
                }
            }
        }

        public void AddUserToRole(ApplicationUser userAccount, string roleName)
        {
            this.AddToRole(userAccount.Id, roleName);
        }


        public void RemoveUser(UserProfile userinfo)
        {
            this.Delete(this.FindById(userinfo.UserId));
        }
        #endregion
    }
}
