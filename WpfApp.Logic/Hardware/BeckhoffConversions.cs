using System;
using System.Collections;
using System.Runtime.InteropServices;
using TwinCAT.PlcOpen;

namespace WpfApp.Logic.Hardware
{
    internal static class BeckhoffConversions
    {
        public static object TryConvertToDotNetManagedType(this object obj) => obj.TryConvertDateTime().TryConvertTimeSpan();

        public static object TryConvertDateTime(this object obj)
        {
            switch (obj)
            {
                case DT date:
                    object targetValue1;
                    if (PlcOpenDTConverter.TryConvert(date, typeof (DateTime), out targetValue1))
                        return targetValue1;
                    break;
                case DATE date:
                    object targetValue2;
                    if (PlcOpenDateConverter.TryConvert((DateBase) date, typeof (DateTime), out targetValue2))
                        return targetValue2;
                    break;
            }
            return obj;
        }

        public static object TryConvertTimeSpan(this object obj)
        {
            switch (obj)
            {
                case TIME time:
                    object targetValue1;
                    if (PlcOpenTimeConverter.TryConvert((TimeBase) time, typeof (TimeSpan), out targetValue1))
                        return targetValue1;
                    break;
                case LTIME ltime:
                    object targetValue2;
                    if (PlcOpenTimeConverter.TryConvert((LTimeBase) ltime, typeof (TimeSpan), out targetValue2))
                        return targetValue2;
                    break;
            }
            return obj;
        }
        
        internal static T ConvertTo<T>(this object obj)
        {
            if (typeof(T) == obj.GetType()) return (T) obj;
            if (((IList) typeof(T).GetInterfaces()).Contains(typeof(IConvertible))) return (T) Convert.ChangeType(obj, typeof(T));
            if (typeof(byte[]) == obj.GetType())
            {
                return (obj as byte[]).ByteArrayToStructure<T>();
            }

            throw new InvalidCastException($"Unable to cast from {obj.GetType()} to {typeof(T)}");
        }
        
        private static T ByteArrayToStructure<T>(this byte[] bytes)
        {
            GCHandle gcHandle = GCHandle.Alloc((object) bytes, GCHandleType.Pinned);
            try
            {
                return (T) Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof (T));
            }
            finally
            {
                gcHandle.Free();
            }
        }
    }
}