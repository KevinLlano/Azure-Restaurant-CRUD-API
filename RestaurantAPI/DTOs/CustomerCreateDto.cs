using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

public class CustomerCreateDto
{
    [Required]
    public string CustomerName { get; set; } = string.Empty;

    // Optional: create orders along with the customer
    public List<OrderCreateDto>? Orders { get; set; }
}