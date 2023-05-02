using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Entities;
using static Play.Inventory.Service.Dtos.InventoryDto;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        public readonly IRepository<InventoryItem> itemsRepository;
        public ItemsController(IRepository<InventoryItem> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();

            var items = (await this.itemsRepository.GetAllAsync(item => item.UserId == userId));

            return Ok(items);
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