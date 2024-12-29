using System;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class UnknownUserException : Exception
    {

        public UnknownUserException(Guid UserId) :
            base($"Unknown User '{UserId}'")
        {
            this.UserId = UserId;
        }

        public Guid UserId { get; set; }
    }
}