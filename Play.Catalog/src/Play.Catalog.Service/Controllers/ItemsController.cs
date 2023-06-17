using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;
using MassTransit;
using Play.Catalog.Contracts;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;

        private readonly IPublishEndpoint publishEndpoint;
        private readonly ILogger<ItemsController> _logger;
        public ItemsController(ILogger<ItemsController> logger,
                                IRepository<Item> itemsRepository,
                                IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var item = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            return Ok(item);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid Id)
        {
            var item = await itemsRepository.GetAsync(Id);
            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateDto(CreateItemDto itemDto)
        {
            var currentItem = new Item
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };


            await itemsRepository.CreateAsync(currentItem);

            await publishEndpoint.Publish(new CatalogItemCreated(currentItem.Id, currentItem.Name, currentItem.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { Id = currentItem.Id }, currentItem);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> PutAsync(Guid Id, UpdateItemDto itemDto)
        {
            var currentItem = await itemsRepository.GetAsync(Id);
            if (currentItem == null)
                return NotFound();

            currentItem.Name = itemDto.Name;
            currentItem.Description = itemDto.Description;
            currentItem.Price = itemDto.Price;

            await itemsRepository.UpdateAsync(currentItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(currentItem.Id, currentItem.Name, currentItem.Description));

            return new NoContentResult();
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAsync(Guid Id)
        {
            var currentItem = await itemsRepository.GetAsync(Id);

            if (currentItem == null)
            {
                return NotFound();
            }


            await itemsRepository.DeleteAsync(currentItem.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(currentItem.Id));

            return new NoContentResult();
        }
    }
}