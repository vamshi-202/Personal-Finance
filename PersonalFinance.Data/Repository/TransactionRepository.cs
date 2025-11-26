using PersonalFinance.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalFinance.Data.Models;


namespace PersonalFinance.Data.Repository
{
  public class TransactionRepository:ITransactionRepository
  {
    private readonly IDbConnection _db;

    public TransactionRepository(IDbConnection db)
    {
      _db = db;
    }

    public async Task<bool> AddTransactionAsync(Transaction transaction)
    {
      try
      {
        await _db.SaveDataAsync("spAddTransaction", new
        {
          transaction.UserId,
          transaction.CategoryId,
          transaction.Amount,
          transaction.Description,
          transaction.TransactionDate
        });

        return true;
      }
      
      catch (Exception ex)
      {
        Console.WriteLine("SQL ERROR: " + ex.Message);
        throw; // Let it throw so you see full stack trace
      }
    }


    public async Task<Transaction?> GetTransactionByTransactionIdAsync(int transactionId) 
    {
      var result = await _db.GetDataAsync<Transaction, dynamic>("spGetTransactionById", new { TransactionId = transactionId });
      return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Transaction>> GetTransactionByUserIdAsync(int userid)
    {
      var result = await _db.GetDataAsync<Transaction, dynamic>("spGetTransactionsByUser", new { UserId = userid });
      return result;
    }

    public async Task<bool> UpdateTransactionAsync(Transaction transaction)
    {
      try
      {
        await _db.SaveDataAsync("spUpdateTransaction", new
        {
          transaction.TransactionId,
          transaction.CategoryId,
          transaction.Amount,
          transaction.Description,
          transaction.TransactionDate
        });

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("SQL Update Error: " + ex.Message);
        return false;
      }
    }


    public async Task<bool> DeleteTransactionAsync(int transactionId)
    {
      try
      {
        var rowsAffected = await _db.SaveDataAsync("spDeleteTransaction", new { TransactionId = transactionId });
        return rowsAffected > 0;
      }
      catch (Exception ex)
      {
        return false;
      }
    }


    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
    int userId,
    string? type,
    DateTime? startDate,
    DateTime? endDate)
    {
      var result = await _db.GetDataAsync<Transaction, dynamic>("spGetTransactionsByUser", new
      {
        UserId = userId,
        Type = type,
        StartDate = startDate,
        EndDate = endDate
      });

      return result;
    }

    public async Task<(decimal income, decimal expense)> GetDashboardSummaryAsync(int userId)
    {
      var result = await _db.GetDataAsync<dynamic, dynamic>("spGetUserDashboardSummary", new { UserId = userId });

      var summary = result.FirstOrDefault();
      if (summary != null)
      {
        return ((decimal)summary.TotalIncome, (decimal)summary.TotalExpense);
      }

      return (0, 0);
    }


  }
}
