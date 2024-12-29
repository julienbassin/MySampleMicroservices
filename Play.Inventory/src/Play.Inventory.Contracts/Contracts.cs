using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play.Inventory.Contracts
{
    public record GrantItems(Guid UserId, 
                            Guid CatalogItemId, 
                            int Quantity, 
                            Guid CorrelationId);

    public record InventoryItemGranted(Guid CorrelationId);

    public record SubstractItems(Guid UserId,
                            Guid CatalogItemId,
                            int Quantity,
                            Guid CorrelationId);

    public record InventoryItemSubstracted(Guid CorrelationId);
}
