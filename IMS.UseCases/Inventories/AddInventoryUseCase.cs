using IMS.CoreBusiness;
using IMS.UseCases.Inventories.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories
{
    public class AddInventoryUseCase : IAddInventoryUseCase
    {
        private IInventoryRepository _inventoryRepository;

        public AddInventoryUseCase(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task ExecuteAsync(Inventory inventory)
        {
            await _inventoryRepository.AddInventoryAsync(inventory);
        }
    }
}
