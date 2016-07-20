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
using System.IO;
using Mono.Data.Sqlite;
//using SQLite;

namespace POSV1.Model
{
    class Item
    {
       //[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string NAME { get; set; }
        public Nullable<double> PurchasedPrice { get; set; }
        public Nullable<double> RetailPrice { get; set; }
        public Nullable<int> AvailableStock { get; set; }
        public Nullable<double> AverageCost { get; set; }
        public Response GetItems(string command, string dbName, int currentItems)
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
                        response.itemList = new List<Item>();
                        contents.CommandText = string.Format(command + " ORDER BY NAME LIMIT {0}, {1};", currentItems, 20);
                        var reader = contents.ExecuteReader();

                        while (reader.Read())
                        {
                            response.itemList.Add(new Item() { Id = Convert.ToInt16(reader["Id"]), 
                                Barcode = reader["Barcode"].ToString(),
                                NAME = reader["NAME"].ToString(),
                                PurchasedPrice = Convert.ToDouble(reader["PurchasedPrice"]),
                                RetailPrice = Convert.ToDouble(reader["RetailPrice"]),
                                AvailableStock = Convert.ToInt16(reader["AvailableStock"]),
                                AverageCost = Convert.ToDouble(reader["AverageCost"])
                            });
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch
            {
                response.message = "Item: Error getting items.";
            }

            return response;
        }
        public Response GetItemCount(string dbName)
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
                        contents.CommandText = string.Format("SELECT Count(*) AS NumberOfItems FROM Item");
                        var reader = contents.ExecuteReader();

                        while (reader.Read())
                        {
                            response.param1 = Convert.ToInt16(reader["NumberOfItems"]);
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch
            {
                response.message = "Item: Error getting item count.";
            }

            return response;
        }
        public Response GetLastId(string dbName)
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
                        contents.CommandText = "SELECT Id FROM Item ORDER BY ID DESC LIMIT 1";
                        var reader = contents.ExecuteReader();

                        while (reader.Read())
                        {
                            response.param1 = Convert.ToInt16(reader["Id"]);
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch
            {
                response.message = "Item: Error getting item count.";
            }

            return response;
        }
    }
}