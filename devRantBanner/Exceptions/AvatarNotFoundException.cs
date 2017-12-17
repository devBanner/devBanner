using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devBanner.Exceptions
{
    public class AvatarNotFoundException : Exception
    {
        public AvatarNotFoundException()
        {
        }

        public AvatarNotFoundException(string username) : base($"Could not find avatar for user {username}!")
        {
        }
    }
}
