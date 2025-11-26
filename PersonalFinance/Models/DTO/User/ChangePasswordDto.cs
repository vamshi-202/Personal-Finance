using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Models.DTO.User
{
  public class ChangePasswordDto
  {
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
  }
}
