using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.CustomExceptions
{
    internal class UserValidationException : Exception
    {
        public UserValidationException()
        {

        }
        public UserValidationException(string message) 
            : base(message)
        {

        }

        public UserValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
