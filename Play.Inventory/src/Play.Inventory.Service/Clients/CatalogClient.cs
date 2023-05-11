using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Play.Inventory.Service.Dtos;
using static Play.Inventory.Service.Dtos.InventoryDto;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        public readonly HttpClient _client;
        public CatalogClient(HttpClient client)
        {
            this._client = client;
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
        {
            var items = await _client.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
            return items;
        }
    }
}