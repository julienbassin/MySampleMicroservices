using System;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class InsufficientFundsException : Exception
    {
        private Guid UserId { get; }
        private decimal GilToDebit { get; }

        public InsufficientFundsException(Guid userId, decimal gil):
            base($"Unsufficient funds for User '{userId}' to debit {gil} ")
        {
            UserId = userId;
            GilToDebit = gil;
        }
    }
}