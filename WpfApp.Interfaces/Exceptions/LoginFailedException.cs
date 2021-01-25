using System;

namespace WpfApp.Interfaces.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException(string message): base(message)
        {
        }
    }
}