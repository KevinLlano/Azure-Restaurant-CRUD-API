using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantAPI.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        // Navigation property for related orders
        public virtual ICollection<OrderMaster> Orders { get; set; } = new List<OrderMaster>();
    }
}
