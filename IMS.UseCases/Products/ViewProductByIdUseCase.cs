using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Products.Interfaces;

namespace IMS.UseCases.Products
{
    public class ViewProductByIdUseCase : IViewProductByIdUseCase
    {
        private readonly IProductRepository _productRepository;

        public ViewProductByIdUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> ExecuteAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }
    }
}
