using System;
using System.Collections.Generic;
using WpfApp.Interfaces.Enums;
using WpfApp.Interfaces.Models;

namespace WpfApp.Interfaces.Services
{
    public interface IUserService
    {
        IObservable<User> CurrentUser { get; }
        void Login(string name, string password);
        bool AddUser(string name, string password, IEnumerable<Role> roles);
        bool UpdateUser(string name, string password, IEnumerable<Role> roles);
        bool RemoveUser(string name);
        void Logout();

    }
}