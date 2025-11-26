using PersonalFinance.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalFinance.Data.Repository
{
  public interface IUserRepository
  {
    Task<bool> CreateUserAsync(User user);
    Task<User?> GetUserByUserIdAsync(int id);
    Task<User?> GetUserByUserNameAsync(string username);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ChangePasswordAsync(int id, string hashedPassword);
  }
}
