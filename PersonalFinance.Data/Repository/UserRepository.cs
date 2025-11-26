using PersonalFinance.Data.Extensions;
using PersonalFinance.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinance.Data.Repository

{
  public class UserRepository: IUserRepository
  {
    private readonly IDbConnection _db; // naming convention for field
    public UserRepository(IDbConnection db) //dependency injection 
    {
      _db = db; // assigning dependency injection
    }

    public async Task<bool> CreateUserAsync(User user)
    {
      try
      {
        await _db.SaveDataAsync("spAddUser", new { Username = user.UserName, user.Email, user.Password });
        return true;
      }
      catch (Exception ex)
      {
        
        Console.WriteLine("Insert error: " + ex.Message);
        throw; // to catch in browser during testing
      }
    }

    public async Task<User?> GetUserByUserIdAsync(int id)
    {
      var result = await _db.GetDataAsync<User, dynamic>("spGetUserById", new { UserId = id });
      return result.FirstOrDefault(); // Returns null if not found
    }

    public async Task<User?> GetUserByUserNameAsync(string username)
    {
      var result = await _db.GetDataAsync<User, dynamic>("spGetUserByUsername", new { UserName = username });
      return result.FirstOrDefault(); // Return null if not found
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
      try
      {
        await _db.SaveDataAsync("spUpdateUser", new
        {
          UserId = user.UserId,
          Username = user.UserName,  //  Map to exact SQL param
          user.Email
        });

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Update failed: " + ex.Message);
        return false;
      }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
      try
      {
        await _db.SaveDataAsync("spDeleteUser", new { UserId = id });
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public async Task<bool> ChangePasswordAsync(int id, string hashedPassword)
    {
      try
      {
        await _db.SaveDataAsync("spChangeUserPassword", new { UserId = id, Password = hashedPassword });
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
