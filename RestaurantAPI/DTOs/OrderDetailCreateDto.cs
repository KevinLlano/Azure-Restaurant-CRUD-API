using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.DTOs;

public class OrderDetailCreateDto
{
    [Required]
    public int FoodItemId { get; set; }

    [Required]
    public decimal FoodItemPrice { get; set; }

    [Required]
    public int Quantity { get; set; }
}