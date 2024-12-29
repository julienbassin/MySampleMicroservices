using System;

namespace Play.Inventory.Service.Exceptions
{
    [Serializable]
    internal class UnknownItemException : Exception
    {
        public UnknownItemException(Guid ItemId):
            base($"Unknown Item '{ItemId}'")
        {
            this.ItemId = ItemId;
        }

        public Guid ItemId { get; }
    }
}