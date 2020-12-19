using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WpfApp.Interfaces.Services
{
    public interface IDatabaseService
    {
        IEnumerable<T> GetCollection<T>(string name, Expression<Func<T, bool>> filter, int skip = 0, int limit = -1) where T : new();
        T InsertIntoCollection<T>(string name, T obj) where T : new();
        T UpdateIntoCollection<T>(string name, T obj) where T : new();
        bool RemoveFromCollection<T>(string name, T obj) where T : new();
        void IndexCollection<T>(string name, Expression<Func<T, string>> index) where T : new();
    }
}