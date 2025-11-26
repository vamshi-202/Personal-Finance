using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Models.DTO.Transaction
{
  public class CreateTransactionDto
  {
    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
  }
}
