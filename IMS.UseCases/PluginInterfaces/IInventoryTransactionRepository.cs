using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IInventoryTransactionRepository
    {
        void PurchaseAsync(string purchaseOrderNumber, Inventory inventory, int quantity, string doneBy, double price);
    }
}