using IMS.WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModelsValidations
{
    public class Produce_EnsureEnoughInventoryQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var produceViewModel = validationContext.ObjectInstance as ProduceViewModel;
            if (produceViewModel != null)
            {
                if (produceViewModel.Product != null && produceViewModel.Product.ProductInventories != null)
                {
                    foreach (var productInventory in produceViewModel.Product.ProductInventories)
                    {
                        if (
                            productInventory.Inventory != null && 
                            productInventory.InventoryQuantity * produceViewModel.QuantityToProduce > productInventory.Inventory.Quantity
                        )
                        {
                            return new ValidationResult(
                                $"The inventory ({productInventory.Inventory.Name}) is not enough to produce {produceViewModel.QuantityToProduce} products.",
                                [validationContext.MemberName!]
                            );
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
