using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces
{
    public interface IDataAccess
    {
        Task<bool> TestConnectionAsync();
        string GetConnectionString(string DB = "default");
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string sql, U param);
        Task<GridReader> LoadMultipleDataAsync<T>(string sql, T param);
        Task<int> SaveDataAsync<T>(string sql, T data);
        Task<T> ExecuteScalarAsync<T, U>(string sql, U param);
        Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param);
    }
}