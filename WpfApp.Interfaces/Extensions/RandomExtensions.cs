using System;

namespace WpfApp.Interfaces.Extensions
{
    public static class RandomExtensions
    {
        public static T GetData<T>(this Random random)
        {
            var type = typeof(T);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return (T)Convert.ChangeType(random.Next() % 2 == 0, type);
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return (T)Convert.ChangeType(random.Next(), type);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return (T)Convert.ChangeType(random.NextDouble(), type);
                case TypeCode.DateTime:
                    return (T)Convert.ChangeType(DateTime.Now, type);
                case TypeCode.String:
                    return (T)Convert.ChangeType(Guid.NewGuid().ToString(), type);
                default:
                    return default(T);
            }
        }
    }
}