using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Models.DTO.Transaction
{
  public class UpdateTransactionDto
  {
    public int TransactionId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public string Description { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime TransactionDate { get; set; }
  }
}
