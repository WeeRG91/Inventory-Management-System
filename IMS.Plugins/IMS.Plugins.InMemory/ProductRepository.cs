using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> _products;

        public ProductRepository()
        {
            _products =
            [
                new() { Id = 1, Name = "Bike", Quantity = 10, Price = 150 },
                new() { Id = 2, Name = "Car", Quantity = 3, Price = 350 },
            ]; 
        }

        public Task AddProductAsync(Product product)
        {
            if (_products.Any(x => x.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            var maxId = _products.Max(x => x.Id);
            product.Id = maxId + 1;

            _products.Add(product);

            return Task.CompletedTask;
        }

        public Task DeleteProductByIdAsync(int id)
        {
            var inventory = _products.FirstOrDefault(x => x.Id == id);
            if (inventory is not null)
            {
                _products.Remove(inventory);
            }

            return Task.CompletedTask;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var product = await Task.FromResult(_products.FirstOrDefault(x => x.Id == id));
            Product? newProduct = null;
            if (product is not null)
            {
                newProduct = new Product();
                newProduct.Id = product.Id;
                newProduct.Name = product.Name;
                newProduct.Price = product.Price;
                newProduct.Quantity = product.Quantity;
                newProduct.ProductInventories = new List<ProductInventory>();
                if (product.ProductInventories != null && product.ProductInventories.Count > 0)
                {
                    foreach(var productInventory in product.ProductInventories)
                    {
                        var newProductInventory = new ProductInventory
                        {
                            InventoryId = productInventory.InventoryId,
                            ProductId = productInventory.ProductId,
                            Product = product,
                            Inventory = new Inventory(),
                            InventoryQuantity = productInventory.InventoryQuantity
                        };
                        if (productInventory.Inventory is not null)
                        {
                            newProductInventory.Inventory.Id = productInventory.Inventory.Id;
                            newProductInventory.Inventory.Name = productInventory.Inventory.Name;
                            newProductInventory.Inventory.Price = productInventory.Inventory.Price;
                            newProductInventory.Inventory.Quantity = productInventory.Inventory.Quantity;
                        }

                        newProduct.ProductInventories.Add(newProductInventory);
                    }
                }
            }

            return await Task.FromResult(newProduct);
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name)) return await Task.FromResult(_products);

            return _products.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public Task UpdateProductAsync(Product product)
        {
            if (_products.Any(x => x.Id != product.Id && x.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask;
            }

            var productToUpdate = _products.FirstOrDefault(x => x.Id == product.Id);
            if (productToUpdate != null)
            {
                productToUpdate.Name = product.Name;
                productToUpdate.Quantity = product.Quantity;
                productToUpdate.Price = product.Price;
                productToUpdate.ProductInventories = product.ProductInventories;
            }

            return Task.CompletedTask;
        }
    }
}
