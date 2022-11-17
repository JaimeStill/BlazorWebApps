using BlazingPizza.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Controllers;

[Route("orders")]
[ApiController]
public class OrdersController : Controller
{
    private readonly PizzaStoreContext _db;

    public OrdersController(PizzaStoreContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _db.Orders
 	        .Include(o => o.Pizzas)
                .ThenInclude(p => p.Special)
 	        .Include(o => o.Pizzas)
                .ThenInclude(p => p.Toppings)
                    .ThenInclude(t => t.Topping)
            .AsSplitQuery()
 	        .OrderByDescending(o => o.CreatedTime)
 	        .ToListAsync();

        return Ok(orders.Select(o => OrderWithStatus.FromOrder(o)).ToList());
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderWithStatus(int orderId)
    {
        var order = await _db.Orders
            .Where(o => o.OrderId == orderId)
            .Include(o => o.Pizzas)
                .ThenInclude(p => p.Special)
            .Include(o => o.Pizzas)
                .ThenInclude(p => p.Toppings)
                    .ThenInclude(t => t.Topping)
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (order == null)
            return NotFound();

        return Ok(OrderWithStatus.FromOrder(order));
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(Order order)
    {
        order.CreatedTime = DateTime.Now;

        // Enforce existence of Pizza.SpecialId and Topping.ToppingId
        // in the database - prevent the submitter from making up
        // new specials and toppings
        foreach (var pizza in order.Pizzas)
        {
            pizza.SpecialId = pizza.Special.Id;
            pizza.Special = null;
        }

        _db.Orders.Attach(order);
        await _db.SaveChangesAsync();

        return Ok(order.OrderId);
    }
}