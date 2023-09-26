using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using static Play.Inventory.Service.Dtos.InventoryDto;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        public readonly IRepository<InventoryItem> itemsRepository;
        public readonly IRepository<CatalogItem> catalogRepository;

        public ItemsController(IRepository<InventoryItem> itemsRepository,
                                IRepository<CatalogItem> catalogRepository)
        {
            this.itemsRepository = itemsRepository;
            this.catalogRepository = catalogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var inventoryitemsEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryitemsEntities.Select(item => item.CatalogItemId);
            var catalogItemEntities = await catalogRepository.GetAllAsync(item => itemIds.Contains(item.Id));


            var inventoryItemDtos = inventoryitemsEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(GrantItemsDto grantItemsDto)
        {

            var inventoryItem = await itemsRepository.GetAsync(item => item.UserId == grantItemsDto.UserId
                                                            && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }

}