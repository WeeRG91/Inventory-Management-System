using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.UseCases.Reports
{
    public class SearchProductTransactionsUseCase : ISearchProductTransactionsUseCase
    {
        private readonly IProductTransactionRepository _productTransactionRepository;

        public SearchProductTransactionsUseCase(IProductTransactionRepository productTransactionRepository)
        {
            _productTransactionRepository = productTransactionRepository;
        }

        public async Task<IEnumerable<ProductTransaction>> ExecuteAsync(
            string name,
            DateTime? dateFrom,
            DateTime? dateTo,
            ProductTransactionType? productTransactionType
        )
        {
            if (dateTo.HasValue) dateTo = dateTo.Value.AddDays(1);

            return await _productTransactionRepository.GetProductTransactionsAsync(name, dateFrom, dateTo, productTransactionType);
        }
    }
}
