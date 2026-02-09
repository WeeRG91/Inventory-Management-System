using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        public List<InventoryTransaction> _inventoryTransactions = new List<InventoryTransaction>();
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryTransactionRepository(IInventoryRepository inventoryRepository) 
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(
            string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? inventoryTransactionType)
        {
            var inventories = (await _inventoryRepository.GetInventoriesByNameAsync(inventoryName)).ToList();

            var query = from invTrans in _inventoryTransactions
                        join inv in inventories on invTrans.InventoryId equals inv.Id
                        where 
                            (string.IsNullOrWhiteSpace(inventoryName) || inv.Name.ToLower().IndexOf(inventoryName.ToLower()) >= 0) &&
                            (!dateFrom.HasValue || invTrans.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || invTrans.TransactionDate <= dateTo.Value.Date) &&
                            (!inventoryTransactionType.HasValue || invTrans.ActivityType == inventoryTransactionType)
                        select new InventoryTransaction
                        {
                            Id = invTrans.Id,
                            Inventory = inv,
                            InventoryId = invTrans.InventoryId,
                            PONumber = invTrans.PONumber,
                            ProductionNumber = invTrans.ProductionNumber,
                            QuantityBefore = invTrans.QuantityBefore,
                            QuantityAfter = invTrans.QuantityAfter,
                            ActivityType = invTrans.ActivityType,
                            TransactionDate = invTrans.TransactionDate,
                            DoneBy = invTrans.DoneBy,
                            UnitPrice = invTrans.UnitPrice,
                        };

            return query;
        }

        public void ProduceAsync(string productionNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            _inventoryTransactions.Add(new InventoryTransaction
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
        }

        public void PurchaseAsync(string purchaseOrderNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            _inventoryTransactions.Add(new InventoryTransaction
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
        }
    }
}
