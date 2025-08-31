using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTOs; // added
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public CustomerController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound();

            return customer;
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Map DTO -> entity graph
            var customer = new Customer
            {
                CustomerName = dto.CustomerName,
                Orders = new List<OrderMaster>()
            };

            if (dto.Orders != null && dto.Orders.Count > 0)
            {
                foreach (var o in dto.Orders)
                {
                    var order = new OrderMaster
                    {
                        OrderNumber = string.IsNullOrWhiteSpace(o.OrderNumber)
                            ? $"ORD-{DateTime.UtcNow.Ticks}"
                            : o.OrderNumber,
                        PMethod = o.PMethod,
                        GTotal = o.GTotal,
                        OrderDetails = new List<OrderDetail>()
                    };

                    if (o.OrderDetails != null && o.OrderDetails.Count > 0)
                    {
                        foreach (var d in o.OrderDetails)
                        {
                            order.OrderDetails.Add(new OrderDetail
                            {
                                // Reference existing FoodItem by id
                                FoodItemId = d.FoodItemId,
                                FoodItemPrice = d.FoodItemPrice,
                                Quantity = d.Quantity
                            });
                        }
                    }

                    customer.Orders.Add(order);
                }
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerID }, customer);
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerID)
                return BadRequest();

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }
    }
}
