using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

public class OrderCreateDto
{
    // Optional; server can generate if omitted
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    public string PMethod { get; set; } = string.Empty;

    [Required]
    public decimal GTotal { get; set; }

    public List<OrderDetailCreateDto>? OrderDetails { get; set; }
}