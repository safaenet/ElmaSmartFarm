﻿using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ElmaSmartFarm.SharedLibrary.Config;
using System.Collections.Generic;
using ElmaSmartFarm.DataLibraryCore.Interfaces;

namespace ElmaSmartFarm.DataLibraryCore.SqlServer
{
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

        public async Task<int> SaveDataAsync<T>(string sql, T data)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            var result = await conn.ExecuteAsync(sql, data);
            return result;
        }

        public async Task<T> ExecuteScalarAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.ExecuteScalarAsync<T>(sql, param);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T, U>(string sql, U param)
        {
            using IDbConnection conn = new SqlConnection(GetConnectionString());
            return await conn.QuerySingleOrDefaultAsync<T>(sql, param);
        }
    }
}