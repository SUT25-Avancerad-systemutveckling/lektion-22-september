using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WontonUpAPI.Data;
using WontonUpAPI.DTOs;
using WontonUpAPI.Models;
using WontonUpAPI.Services;

namespace WontonUpAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersDbContext _db;
        private readonly MenuService _menu;

        public OrdersController(OrdersDbContext db, MenuService menu)
        {
            _db = db;
            _menu = menu;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _db.Orders.AsNoTracking().ToListAsync();
            var result = orders.Select(order => MapToOrderDto(order));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound(new { error = "NotFound", message = "Order not found" });
            return Ok(MapToOrderDto(order));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto body)
        {
            if (body == null || body.Items == null || body.Items.Length == 0)
                return BadRequest(new { error = "BadRequest", message = "Items are required" });

            // Validate item ids
            var menuItems = body.Items.Select(id => _menu.GetById(id)).ToArray();
            if (menuItems.Any(menuItem => menuItem == null))
            {
                return BadRequest(new { error = "BadRequest", message = "One or more item ids are invalid" });
            }

            // Aggregate quantities
            var qtyById = body.Items.GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count());

            var ordered = qtyById.Select(kv =>
            {
                var menu = _menu.GetById(kv.Key)!;
                return new OrderedItemDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    Type = menu.Type,
                    Quantity = kv.Value,
                    Price = menu.Price * kv.Value
                };
            }).ToArray();

            var orderValue = ordered.Sum(i => i.Price);

            var timestamp = DateTime.UtcNow;
            var eta = timestamp.AddMinutes(15);

            var orderId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                ItemsJson = JsonSerializer.Serialize(ordered),
                OrderValue = orderValue,
                Timestamp = timestamp,
                Eta = eta,
                State = "waiting"
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            TimeSpan etaToDelivery = eta - timestamp;

            var receipt = new ReceiptDto
            {
                Id = orderId,
                OrderValue = orderValue,
                Timestamp = timestamp.ToString(),
                Eta = (int)etaToDelivery.TotalMinutes,
                Items = ordered.Select(i => new OrderedReceiptItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Type = i.Type,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToArray()
            };

            return CreatedAtAction(nameof(Get), new { id = orderId }, receipt);
        }

        private static OrderDto MapToOrderDto(Order o)
        {
            var items = JsonSerializer.Deserialize<OrderedItemDto[]>(o.ItemsJson) ?? Array.Empty<OrderedItemDto>();
            return new OrderDto
            {
                Id = o.Id,
                Items = items.Select(i => new OrderedReceiptItemDto { Id = i.Id, Name = i.Name, Type = i.Type, Quantity = i.Quantity, Price = i.Price }).ToArray(),
                OrderValue = o.OrderValue,
                Eta = o.Eta.ToString("o"),
                Timestamp = o.Timestamp.ToString("o"),
                State = o.State
            };
        }
    }
}
