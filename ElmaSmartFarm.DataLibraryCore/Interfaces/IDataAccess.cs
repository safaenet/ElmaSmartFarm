using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmaSmartFarm.DataLibraryCore.Interfaces;

public interface IDataAccess
{
    Task<bool> TestConnectionAsync();
    string GetConnectionString(string DB = "default");
    Task<IEnumerable<T>> LoadDataAsync<T, U>(string sql, U param);
    IEnumerable<T> LoadData<T, U>(string sql, U param);
    Task<IEnumerable<T>> LoadDataAsync<T>(string sql);
    IEnumerable<T> LoadData<T>(string sql);
    Task<int> SaveDataAsync<T>(string sql, T data);
    int SaveData<T>(string sql, T data);
    Task<T> ExecuteScalarAsync<T, U>(string sql, U param);
    Task<T> ExecuteScalarAsync<T>(string sql);
    Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param);
    Task<T> QuerySingleOrDefaultAsync<T>(string sql);
}