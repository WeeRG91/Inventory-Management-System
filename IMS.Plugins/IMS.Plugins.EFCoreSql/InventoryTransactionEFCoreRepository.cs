using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class InventoryTransactionEFCoreRepository : IInventoryTransactionRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public InventoryTransactionEFCoreRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(
            string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            var query = from invTrans in db.InventoryTransactions
                        join inv in db.Inventories on invTrans.InventoryId equals inv.Id
                        where
                            (string.IsNullOrWhiteSpace(inventoryName) || inv.Name.ToLower().IndexOf(inventoryName.ToLower()) >= 0) &&
                            (!dateFrom.HasValue || invTrans.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || invTrans.TransactionDate <= dateTo.Value.Date) &&
                            (!inventoryTransactionType.HasValue || invTrans.ActivityType == inventoryTransactionType)
                        select invTrans;

            return await query.Include(x => x.Inventory).ToListAsync();
        }

        public async Task ProduceAsync(string productionNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            db.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.Id,
                QuantityBefore = inventory.Quantity,
                QuantityAfter = inventory.Quantity - quantity,
                ActivityType = InventoryTransactionType.ProduceProduct,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = price,
            });

            await db.SaveChangesAsync();
        }

        public async Task PurchaseAsync(string purchaseOrderNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            db.InventoryTransactions.Add(new InventoryTransaction
            {
                PONumber = purchaseOrderNumber,
                InventoryId = inventory.Id,
                QuantityBefore = inventory.Quantity,
                QuantityAfter = inventory.Quantity + quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                TransactionDate = DateTime.UtcNow,
                DoneBy = doneBy,
                UnitPrice = price
            });

            await db.SaveChangesAsync();
        }
    }
}

