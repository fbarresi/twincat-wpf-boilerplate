using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using System.Text;
using Ninject;
using Serilog;
using Serilog.Debugging;
using WpfApp.Interfaces.Enums;
using WpfApp.Interfaces.Exceptions;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;
using WpfApp.Interfaces.Settings;

namespace WpfApp.Logic.Services
{
    public class UserService : IUserService, IDisposable, IInitializable
    {
        private const string CollectionName = "Users";
        private readonly DatabaseService databaseService;
        private readonly ApplicationSetting setting;
        private readonly ILogger logger;

        private readonly BehaviorSubject<User> currentUserSubject = new BehaviorSubject<User>(null);
        private readonly SerialDisposable disposable = new SerialDisposable();
        public UserService(DatabaseService databaseService, ApplicationSetting setting, ILogger logger)
        {
            this.databaseService = databaseService;
            this.setting = setting;
            this.logger = logger;
        }

        public IObservable<User> CurrentUser => currentUserSubject.AsObservable();
        public void Login(string name, string password)
        {
            var hash = ComputeSha256Hash(password);
            var retrievedUser = databaseService.GetCollection<User>(CollectionName, 
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && user.PasswordHash.Equals(hash))
                .SingleOrDefault();
            if (retrievedUser == null)
            {
                logger.Debug("Invalid username or password by login");
                throw new LoginFailedException("Invalid username of password");
            } 
            
            logger.Debug("Login with user {Name}", name);
            
            currentUserSubject.OnNext(retrievedUser);

            StartAutologout();
        }

        public bool AddUser(string name, string password, IEnumerable<Role> roles)
        {
            logger.Debug("Try adding user {Name} ...", name);
            if (databaseService.GetCollection<User>(CollectionName,
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Any())
            {
                logger.Debug("An user with name {Name} already exists", name);
                return false;
            }
            databaseService.InsertIntoCollection(CollectionName,
                new User() {Name = name, PasswordHash = ComputeSha256Hash(password), Roles = roles.ToList()});
            
            logger.Debug("User {Name} added", name);

            return true;
        }

        public bool UpdateUser(string name, string password, IEnumerable<Role> roles)
        {
            logger.Debug("Try updating user {Name} ...", name);
            if (databaseService.GetCollection<User>(CollectionName,
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .Any())
            {
                var retrievedUser = databaseService.GetCollection<User>(CollectionName, 
                        user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();

                if (retrievedUser == null)
                {
                    logger.Debug("More than one user with name {Name} exists", name);
                    return false;
                }
                
                retrievedUser.PasswordHash = ComputeSha256Hash(password);
                retrievedUser.Roles = roles.ToList();
                databaseService.UpdateIntoCollection(CollectionName, retrievedUser);
                logger.Debug("User {Name} updated", name);
                return true;
            }
            logger.Debug("User {Name} do not exists", name);

            return false;
        }

        public bool RemoveUser(string name)
        {
            logger.Debug("Try removing user {Name} from database...", name);
            var retrievedUser = databaseService.GetCollection<User>(CollectionName, 
                    user => user.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefault();
            if (retrievedUser != null)
            {
                databaseService.RemoveFromCollection(CollectionName, retrievedUser);
                logger.Debug("User {Name} removed", name);
                return true;
            }
            return false;
        }

        private void StartAutologout()
        {
            logger.Debug("Auto logout programmed in {Time}", setting.Autologout);
            disposable.Disposable = Observable.Timer(setting.Autologout)
                .Do(_ => Logout())
                .Subscribe()
                ;
        }

        public void Logout()
        {
            if(currentUserSubject.Value != null)
                logger.Debug("Logout for {Name}", currentUserSubject.Value.Name);
            currentUserSubject.OnNext(null);
            disposable.Disposable = null;
        }

        public void Dispose()
        {
            currentUserSubject?.Dispose();
            disposable?.Dispose();
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

        public void Initialize()
        {
            var users = databaseService.CountElementInCollection<User>(CollectionName);
            if (users == 0)
                SetDefaultUsers();
        }

        private void SetDefaultUsers()
        {
            logger.Information("Adding default user to application");
            databaseService.InsertIntoCollection(CollectionName, new User()
            {
                Name = "root", 
                PasswordHash = ComputeSha256Hash("root"), 
                Roles = new List<Role>()
                {
                    Role.Root
                }
            });
        }
    }
}