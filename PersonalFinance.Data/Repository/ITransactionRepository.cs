using PersonalFinance.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinance.Data.Repository
{
  public interface ITransactionRepository
  {
    Task<bool> AddTransactionAsync(Transaction transaction);
    Task<Transaction?> GetTransactionByTransactionIdAsync(int transactionId);
    Task<IEnumerable<Transaction>> GetTransactionByUserIdAsync(int userId);
    Task<bool> UpdateTransactionAsync(Transaction transaction);
    Task<bool> DeleteTransactionAsync(int transactionId);
    Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(int userId, string? type, DateTime? startDate, DateTime? endDate);
    Task<(decimal income, decimal expense)> GetDashboardSummaryAsync(int userId);


  }
}
