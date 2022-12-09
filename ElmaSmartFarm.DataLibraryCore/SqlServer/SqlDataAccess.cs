using Dapper;
using ElmaSmartFarm.DataLibraryCore.Interfaces;
using ElmaSmartFarm.SharedLibrary.Config;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ElmaSmartFarm.DataLibraryCore.SqlServer;

public class SqlDataAccess : IDataAccess
{
    public string GetConnectionString(string DB = "default") => SettingsDataAccess.AppConfiguration().GetConnectionString(DB);

    private bool TestConnection()
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        System.Data.Common.DbConnectionStringBuilder csb = new();
        try
        {
            csb.ConnectionString = GetConnectionString();
            conn.Open();
        }
        catch
        {
            return false;
        }
        return true;
    }

    public async Task<bool> TestConnectionAsync()
    {
        var task = Task.Run(() => TestConnection());
        return await task;
    }

    public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string sql, U param)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        return await conn.QueryAsync<T>(sql, param);
    }

    public async Task<IEnumerable<T>> LoadDataAsync<T>(string sql)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        return await conn.QueryAsync<T>(sql);
    }

    public async Task<int> SaveDataAsync<T>(string sql, T data)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        var result = await conn.ExecuteAsync(sql, data);
        return result;
    }

    public int SaveData<T>(string sql, T data)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        var result = conn.Execute(sql, data);
        return result;
    }

    public async Task<T> ExecuteScalarAsync<T, U>(string sql, U param)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        return await conn.ExecuteScalarAsync<T>(sql, param);
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        return await conn.ExecuteScalarAsync<T>(sql);
    }

    public async Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
    }

    public async Task<T> QuerySingleOrDefaultAsync<T>(string sql)
    {
        using IDbConnection conn = new SqlConnection(GetConnectionString());
        return await conn.QuerySingleOrDefaultAsync<T>(sql);
    }
}