using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PersonalFinance.Data.Extensions
{
    public static class SqlDataAccessExtensions
    {
        // For SELECT queries
        public static async Task<IEnumerable<T>> GetDataAsync<T, P>( this IDbConnection db, string spName, P parameters , CommandType commandType = CommandType.StoredProcedure)
        {
            return await db.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
        }

        // For INSERT, UPDATE, DELETE
        public static async Task<int> SaveDataAsync<T>(this IDbConnection db, string spName, T parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            return await db.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
