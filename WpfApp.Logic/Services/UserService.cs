using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using System.Text;
using Serilog.Debugging;
using WpfApp.Interfaces.Enums;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Logic.Services
{
    public class UserService : IUserService, IDisposable
    {
        private const string CollectionName = "Users";
        private readonly DatabaseService databaseService;
        private readonly ApplicationSetting setting;

        private readonly BehaviorSubject<User> currentUserSubject = new BehaviorSubject<User>(null);
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        public UserService(DatabaseService databaseService, ApplicationSetting setting)
        {
            this.databaseService = databaseService;
            this.setting = setting;
        }

        public IObservable<User> CurrentUser => currentUserSubject.AsObservable();
        public void Login(string name, string password)
        {
            var hash = ComputeSha256Hash(password);
            var retrievedUser = databaseService.GetCollection<User>(CollectionName, 
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && user.PasswordHash.Equals(hash))
                .SingleOrDefault();
            if (retrievedUser == null) throw new LoggingFailedException("Invalid username of password");
            
            currentUserSubject.OnNext(retrievedUser);

            StartAutologout();
        }

        public bool AddUser(string name, string password, IEnumerable<Role> roles)
        {
            if (databaseService.GetCollection<User>(CollectionName,
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Any()) return false;
            databaseService.InsertIntoCollection(CollectionName,
                new User() {Name = name, PasswordHash = ComputeSha256Hash(password), Roles = roles.ToList()});
            return true;
        }

        public bool UpdateUser(string name, string password, IEnumerable<Role> roles)
        {
            if (databaseService.GetCollection<User>(CollectionName,
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Any())
            {
                var retrievedUser = databaseService.GetCollection<User>(CollectionName, 
                        user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();
                
                if (retrievedUser == null) 
                    return false;
                
                retrievedUser.PasswordHash = ComputeSha256Hash(password);
                retrievedUser.Roles = roles.ToList();
                databaseService.UpdateIntoCollection(CollectionName, retrievedUser);
                return true;
            }

            return false;
        }

        public bool RemoveUser(string name)
        {
            var retrievedUser = databaseService.GetCollection<User>(CollectionName, 
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault();
            if (retrievedUser != null)
            {
                databaseService.RemoveFromCollection(CollectionName, retrievedUser);
                return true;
            }
            return false;
        }

        private void StartAutologout()
        {
            Observable.Timer(setting.Autologout)
                .Do(_ => Logout())
                .Subscribe()
                .AddDisposableTo(disposables)
                ;
        }

        private void Logout()
        {
            currentUserSubject.OnNext(null);
        }

        public void Dispose()
        {
            currentUserSubject?.Dispose();
            disposables?.Dispose();
        }
        
        private string ComputeSha256Hash(string plainText)  
        {  
            // Create a SHA256   
            using (var sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
        }  
    }
}