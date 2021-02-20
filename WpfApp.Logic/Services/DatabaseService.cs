using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;
using Ninject;
using WpfApp.Interfaces.Extensions;
using WpfApp.Interfaces.Models;
using WpfApp.Interfaces.Services;

namespace WpfApp.Logic.Services
{
    public class DatabaseService : IInitializable, IDatabaseService, IDisposable
    {
        private LiteDatabase db;
        private IDirectoryService directoryService;

        public DatabaseService(IDirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }
        

        public void Initialize()
        {
            db = new LiteDatabase(Path.Combine(directoryService.DatabaseFolder, "local.db"));
        }


        public IEnumerable<T> GetCollection<T>(string name, Expression<Func<T, bool>> filter, int skip = 0, int limit = -1) where T : new()
        {
            var collection = GetDbCollection<T>(name);
            var found = collection.Find(filter, skip, limit>1 ? limit : int.MaxValue);
            return found;
        }

        public int CountElementInCollection<T>(string name) where T : new() => GetDbCollection<T>(name).Count();

        public T InsertIntoCollection<T>(string name, T obj) where T : new()
        {
            var collection = GetDbCollection<T>(name);
            collection.Insert(obj);
            return obj;
        }

        public T UpdateIntoCollection<T>(string name, T obj) where T : new()
        {
            var collection = GetDbCollection<T>(name);
            collection.Update(obj);
            return obj;
        }

        public bool RemoveFromCollection<T>(string name, T obj) where T : new()
        {
            var collection = GetDbCollection<T>(name);
            var objectId = (obj as DatabaseObjectBase)?.Id;
            if (objectId.HasValue)
                return collection.Delete(objectId.Value);
            return false;
        }

        private ILiteCollection<T> GetDbCollection<T>(string name) where T : new()
        {
            return db.GetCollection<T>(name);
        }

        public void IndexCollection<T>(string name, Expression<Func<T, string>> index) where T : new()
        {
            var collection = GetDbCollection<T>(name);
            collection.EnsureIndex(index);
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}