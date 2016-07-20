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
using System.Data;
using System.IO;

namespace POSV1.Model
{
    class Configuration
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int IsGranted { get; set; }

        public Response GetAllConfiguration(string dbName)
        {
            Response response = new Response();
            response.status = "FAILURE";
            var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var pathToDatabase = Path.Combine(documents, dbName);

            try
            {
                var connectionString = String.Format("Data Source={0}", pathToDatabase);
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var contents = conn.CreateCommand())
                    {
                        response.configurationList = new List<Configuration>();
                        contents.CommandText = "SELECT * FROM Configuration;";
                        var reader = contents.ExecuteReader();

                        while (reader.Read())
                        {
                            response.configurationList.Add(new Configuration() { Id = Convert.ToInt16(reader["Id"]), Description = reader["Description"].ToString(), IsGranted = Convert.ToInt16(reader["IsGranted"]) });
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch
            {
                response.message = "Configuration: Error getting access.";
            }

            return response;
        }
    }
}