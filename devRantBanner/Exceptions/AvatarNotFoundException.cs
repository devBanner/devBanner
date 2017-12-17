using System;

namespace devBanner.Exceptions
{
    public class AvatarNotFoundException : Exception
    {
        public AvatarNotFoundException()
        {
        }

        public AvatarNotFoundException(string username) : base($"Could not find avatar for {username}!")
        {
        }
    }
}
