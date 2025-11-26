using System.ComponentModel.DataAnnotations;

namespace PersonalFinance.Models.DTO.User
{
  public class UpdateUserDto
  {
    [Required]
    public int UserId { get; set; }  // Needed to identify which user to update

    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}
