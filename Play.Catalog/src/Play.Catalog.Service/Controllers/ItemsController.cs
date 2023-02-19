using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        public static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poinson", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Golden sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        };
        private readonly ILogger<ItemsController> _logger;
        public ItemsController(ILogger<ItemsController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public IEnumerable<ItemDto> GetItems()
        {
            return items;
        }

        [HttpGet("{Id}")]
        public ItemDto GetItemById(Guid Id)
        {
            var item = items.Where(i => i.Id == Id).SingleOrDefault();
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return item;
        }

        [HttpPost]
        public ActionResult<ItemDto> CreateDto(CreateItemDto itemDto)
        {
            var currentItem = new ItemDto(Guid.NewGuid(), itemDto.Name, itemDto.Description, itemDto.Price, DateTimeOffset.UtcNow);
            items.Add(currentItem);
            return CreatedAtAction(nameof(GetItemById), new { Id = currentItem.Id }, currentItem);
        }
    }
}