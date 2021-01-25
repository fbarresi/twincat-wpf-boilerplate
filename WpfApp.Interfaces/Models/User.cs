using System.Collections.Generic;
using WpfApp.Interfaces.Enums;

namespace WpfApp.Interfaces.Models
{
    public class User : DatabaseObjectBase
    {
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public List<Role> Roles { get; set; }

        bool HasRole(Role role)
        {
            return Roles?.Contains(role) ?? false;
        }
    }
}