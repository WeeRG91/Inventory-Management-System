using IMS.CoreBusiness;

namespace IMS.UseCases.Reports.Interfaces
{
    public interface ISearchProductTransactionsUseCase
    {
        Task<IEnumerable<ProductTransaction>> ExecuteAsync(string name, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? productTransactionType);
    }
}