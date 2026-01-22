using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class InventoryRepository : IInventoryRepository
    {
        private List<Inventory> _inventories;

        public InventoryRepository()
        {
            _inventories =
            [
                new Inventory { Id = 1, Name = "Bike Seat", Quantity = 10, Price = 2 },
                new Inventory { Id = 2, Name = "Bike Body", Quantity = 10, Price = 15 },
                new Inventory { Id = 3, Name = "Bike Wheels", Quantity = 20, Price = 8 },
                new Inventory { Id = 4, Name = "Bike Padels", Quantity = 20, Price = 2 },
            ];
        }

        public Task AddInventoryAsync(Inventory inventory)
        {
            if (_inventories.Any(x => x.Name.Equals(inventory.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask; 
            }

            var maxId = _inventories.Max(x => x.Id);
            inventory.Id = maxId + 1;

            _inventories.Add(inventory);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return await Task.FromResult(_inventories);

            return _inventories.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
