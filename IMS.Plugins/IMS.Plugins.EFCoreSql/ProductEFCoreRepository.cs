using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSql
{
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public ProductEFCoreRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task AddProductAsync(Product product)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            db.Products.Add(product);
            FlagInventoryUnchanged(product, db);
            await db.SaveChangesAsync();
        }

        public async Task DeleteProductByIdAsync(int id)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            var product = db.Products.FirstOrDefault(x => x.Id == id);
            if (product == null) return;

            db.Products.Remove(product);
            await db.SaveChangesAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            Product? product = await db.Products
                .Include(x => x.ProductInventories!).ThenInclude(x => x.Inventory).FirstOrDefaultAsync(x => x.Id == id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();

            //if (string.IsNullOrWhiteSpace(name))
            //    return Enumerable.Empty<Product>();

            return await db.Products
                .Where(x => EF.Functions.Like(x.Name, $"%{name.Trim()}%"))
                .OrderBy(x => x.Name)
                //.Take(20)
                .ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            using AppDbContext db = _dbContextFactory.CreateDbContext();
            var productToUpdate = await db.Products.Include(x => x.ProductInventories!).FirstOrDefaultAsync(x => x.Id == product.Id);
            if (productToUpdate != null)
            {
                productToUpdate.Name = product.Name;
                productToUpdate.Quantity = product.Quantity;
                productToUpdate.Price = product.Price;
                productToUpdate.ProductInventories = product.ProductInventories;
                FlagInventoryUnchanged(product, db);
                await db.SaveChangesAsync();
            }
        }

        private void FlagInventoryUnchanged(Product product, AppDbContext db)
        {
            if (product.ProductInventories != null)
            {
                foreach (var productInventory in product.ProductInventories)
                {
                    // Check if ProductInventory is already tracked
                    var trackedPI = db.ChangeTracker.Entries<ProductInventory>()
                                      .FirstOrDefault(e =>
                                          e.Entity.ProductId == productInventory.ProductId &&
                                          e.Entity.InventoryId == productInventory.InventoryId);

                    if (trackedPI == null)
                    {
                        // Attach ProductInventory
                        db.Entry(productInventory).State = EntityState.Unchanged;
                    }

                    // Also handle Inventory entity safely
                    var inventory = productInventory.Inventory;
                    if (inventory != null)
                    {
                        var trackedInventory = db.ChangeTracker.Entries<Inventory>()
                                                 .FirstOrDefault(e => e.Entity.Id == inventory.Id);
                        if (trackedInventory == null)
                        {
                            db.Entry(inventory).State = EntityState.Unchanged;
                        }
                    }
                }
            }
        }
    }
}
