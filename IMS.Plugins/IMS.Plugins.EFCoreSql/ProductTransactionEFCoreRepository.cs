using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class ProductTransactionEFCoreRepository : IProductTransactionRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public ProductTransactionEFCoreRepository(
            IDbContextFactory<AppDbContext> dbContextFactory,
            IProductRepository productRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository
        )
        {
            _dbContextFactory = dbContextFactory;
            _productRepository = productRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            var productToProduce = await _productRepository.GetProductByIdAsync(product.Id);
            if (productToProduce != null)
            {
                if (productToProduce.ProductInventories != null)
                {
                    foreach (var productInventory in productToProduce.ProductInventories)
                    {
                        if (productInventory.Inventory != null)
                        {
                            //Add inventory transaction
                            await _inventoryTransactionRepository.ProduceAsync(
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
                                await _inventoryRepository.UpdateInventoryAsync(inventory);
                            }

                        }

                    }
                }

            }

            //Add product transaction
            db.ProductTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.Id,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity + quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
            });

            await db.SaveChangesAsync();
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            db.ProductTransactions.Add(new ProductTransaction
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

           await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionsAsync(string name, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            var query = from proTrans in db.ProductTransactions
                        join pro in db.Products on proTrans.ProductId equals pro.Id
                        where
                            (string.IsNullOrWhiteSpace(name) || pro.Name.ToLower().IndexOf(name.ToLower()) >= 0) &&
                            (!dateFrom.HasValue || proTrans.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || proTrans.TransactionDate <= dateTo.Value.Date) &&
                            (!productTransactionType.HasValue || proTrans.ActivityType == productTransactionType)
                        select proTrans;

            return await query.Include(x => x.Product).ToListAsync();
        }
    }
}
