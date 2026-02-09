using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new List<ProductTransaction>();
        private readonly IProductRepository _productRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public ProductTransactionRepository(
            IProductRepository productRepository, 
            IInventoryTransactionRepository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository
        )
        {
            _productRepository = productRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            var productToUpdate = await _productRepository.GetProductByIdAsync(product.Id);
            if (productToUpdate != null)
            {
                if (productToUpdate.ProductInventories != null)
                {
                    foreach (var productInventory in productToUpdate.ProductInventories)
                    {
                        if (productInventory.Inventory != null)
                        {
                            //Add inventory transaction
                            _inventoryTransactionRepository.ProduceAsync(
                                productionNumber, 
                                productInventory.Inventory, 
                                productInventory.InventoryQuantity * quantity, 
                                doneBy,
                                productInventory.Inventory.Price
                            );

                            // Decrease the inventories
                            var inventory = await _inventoryRepository.GetInventoryByIdAsync(productInventory.InventoryId);
                            if (inventory != null)
                            {
                                inventory.Quantity -= productInventory.InventoryQuantity * quantity;
                                await _inventoryRepository.UpdateInventoryAsync( inventory );
                            }
                            
                        }
                        
                    }
                }
                
            }

            //Add product transaction
            _productTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.Id,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity + quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
            });
        }

        public Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            _productTransactions.Add(new ProductTransaction
            {
                ActivityType = ProductTransactionType.SellProduct,
                SONumber = salesOrderNumber,
                ProductId = product.Id,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = unitPrice,
            });

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string name, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType)
        {
            var products = (await _productRepository.GetProductsByNameAsync(name)).ToList();

            var query = from proTrans in _productTransactions
                        join pro in products on proTrans.ProductId equals pro.Id
                        where
                            (string.IsNullOrWhiteSpace(name) || pro.Name.ToLower().IndexOf(name.ToLower()) >= 0) &&
                            (!dateFrom.HasValue || proTrans.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || proTrans.TransactionDate <= dateTo.Value.Date) &&
                            (!productTransactionType.HasValue || proTrans.ActivityType == productTransactionType)
                        select new ProductTransaction
                        {
                            Id = proTrans.Id,
                            Product = pro,
                            ProductId = proTrans.ProductId,
                            ProductionNumber = proTrans.ProductionNumber,
                            SONumber = proTrans.SONumber,
                            QuantityBefore = proTrans.QuantityBefore,
                            QuantityAfter = proTrans.QuantityAfter,
                            ActivityType = proTrans.ActivityType,
                            TransactionDate = proTrans.TransactionDate,
                            DoneBy = proTrans.DoneBy,
                            UnitPrice = proTrans.UnitPrice,
                        };

            return query;
        }
    }
}
