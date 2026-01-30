using IMS.CoreBusiness.Validations;
using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness
{
    public class Product
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

        [Product_EnsurePriceIsGreaterThanInventoriesCost]
        public List<ProductInventory>? ProductInventories { get; set; } = new List<ProductInventory>();

        public void AddInventory(Inventory inventory)
        {
            if (
                this.ProductInventories is not null && 
                !this.ProductInventories.Any(x => x.Inventory is not null && x.Inventory.Name.Equals(inventory.Name))
                )
            {
                this.ProductInventories.Add(new ProductInventory
                {
                    InventoryId = inventory.Id,
                    Inventory = inventory,
                    InventoryQuantity = 1,
                    ProductId = this.Id,
                    Product = this
                });
            }
            
        }

        public void RemoveInventory(ProductInventory productInventory)
        {
            this.ProductInventories?.Remove(productInventory);
        }
    }
}
