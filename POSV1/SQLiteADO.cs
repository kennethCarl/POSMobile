using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

namespace POSV1
{
    class SQLiteADO
    {
        public Response CreateDatabase(string dbName)
        {
            Response response = new Response();
            response.status = "FAILURE";
            try
            {
                var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var pathToDatabase = Path.Combine(documents, dbName);
                SqliteConnection.CreateFile(pathToDatabase);
                response.status = "SUCCESS";
            }
            catch(Exception e)
            {
                response.message = "Error Creating Database.";
            }
            return response;
        }
        public Response CreateTable(string command, string pathToDatabase)
        {
            Response response = new Response();
            response.status = "FAILURE";
            try
            {
                var connectionString = String.Format("Data Source={0};Version=3;", pathToDatabase);
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = command;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    response.status = "SUCCESS";
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                response.message = "Error Creating Table to Database.";
            }
            return response;
        }
        public Response ExecuteNonQuery(string command, string path)
        {
            Response response = new Response();
            response.status = "FAILURE";
            try
            {
                var connectionString = String.Format("Data Source={0}", path);
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = command;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                
                response.status = "SUCCESS";
            }
            catch (Exception e)
            {
                response.message = "Error Executing Non-query.";
            }
            return response;
        }
    }
}