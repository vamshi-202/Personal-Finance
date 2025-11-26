using PersonalFinance.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinance.Data.Repository
{
  public interface ICategoryRepository
  {
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string type);
  }
}
