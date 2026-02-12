using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public InventoryEFCoreRepository(IDbContextFactory<AppDbContext> dbContextFactory) 
        { 
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            db.Inventories.Add(inventory);
            await db.SaveChangesAsync();
        }

        public async Task DeleteInventoryByIdAsync(int id)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            var inventory = db.Inventories.FirstOrDefault(x => x.Id == id);
            if (inventory == null) return;
            db.Inventories.Remove(inventory);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            //if (string.IsNullOrWhiteSpace(name))
            //    return Enumerable.Empty<Inventory>();

            return await db.Inventories
                .Where(x => EF.Functions.Like(x.Name, $"%{name.Trim()}%"))
                .OrderBy(x => x.Name)
                //.Take(20)
                .ToListAsync();
        }

        public async Task<Inventory?> GetInventoryByIdAsync(int id)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            var inventory = await db.Inventories.FindAsync(id);
            return inventory;

        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            var inventoryToUpdate = await db.Inventories.FindAsync(inventory.Id);
            if (inventoryToUpdate != null)
            {
                inventoryToUpdate.Name = inventory.Name;
                inventoryToUpdate.Quantity = inventory.Quantity;
                inventoryToUpdate.Price = inventory.Price;
                await db.SaveChangesAsync();
            }
        }
    }
}
