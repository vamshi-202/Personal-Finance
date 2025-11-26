using PersonalFinance.Data.Extensions;
using PersonalFinance.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinance.Data.Repository
{
  public class CategoryRepository:ICategoryRepository
  {
    private readonly IDbConnection _db;

    public CategoryRepository(IDbConnection db)
    {
      _db = db;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    { 
        return  await _db.GetDataAsync<Category, dynamic>("spGetAllCategories", new { });
    }

    public async Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string type)
    {
      try
      {
        return await _db.GetDataAsync<Category, dynamic>("spGetCategoriesByType",new { Type = type });
      }
      catch (Exception)
      {
        return Enumerable.Empty<Category>(); // Return empty list on failure
      }
    }

  }
}
