using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinance.Data.Models
{
  public class Category
  {
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string Type { get; set; } // "Income" or "Expense"
  }
}
