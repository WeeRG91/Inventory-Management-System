using IMS.UseCases.Inventories.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Inventories
{
    public class DeleteInventoryUseCase : IDeleteInventoryUseCase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public DeleteInventoryUseCase(IInventoryRepository inventoryRepository)
        {
            this._inventoryRepository = inventoryRepository;
        }
        public async Task ExecuteAsync(int id)
        {
            await _inventoryRepository.DeleteInventoryByIdAsync(id);
        }
    }
}
