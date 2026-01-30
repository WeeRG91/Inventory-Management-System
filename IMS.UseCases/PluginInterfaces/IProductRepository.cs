using IMS.CoreBusiness;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductRepository
    {
        
        Task AddProductAsync(Product product);
        Task DeleteProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
        Task<Product?> GetProductByIdAsync(int id);
        Task UpdateProductAsync(Product product);
    }
}
