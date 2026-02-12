using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModels
{
    public class InventoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantiy must be equal or greater than 0.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be equal or greater than 0.")]
        public double Price { get; set; }
    }
}
