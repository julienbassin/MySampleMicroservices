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
namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private static int requestCounter = 0;
        private readonly ILogger<ItemsController> _logger;
        public ItemsController(ILogger<ItemsController> logger,
                                IRepository<Item> itemsRepository)
        {
            _logger = logger;
            this.itemsRepository = itemsRepository;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            requestCounter++;
            Console.WriteLine($"Request {requestCounter}: Starting...");

            if (requestCounter <= 2)
            {
                Console.WriteLine($"Request {requestCounter}: Delaying...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if (requestCounter <= 4)
            {
                Console.WriteLine($"Request {requestCounter}: 500 (Internal Server Error). ");
                return StatusCode(500);
            }

            var item = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            Console.WriteLine($"Request {requestCounter}: 200 (Ok). ");
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

            return new NoContentResult();
        }
    }
}