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
    class SaleMaster
    {
        //[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime SalesDateTime { get; set; }
        public double Amount { get; set; }
        public double AmountReceived { get; set; }
        public string SoldBy { get; set; }
        public string SoldTo { get; set; }
        public Response getSalesByDate(DateTime date, string dbName)
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
                        contents.CommandText = string.Format("SELECT SaleMaster.SalesDateTime " +
                                            ", (SELECT SUM(Amount) FROM SaleMaster WHERE SalesDateTime BETWEEN datetime('{0}') AND datetime('{1}')) AS TotalAmount " +
                                            ", COUNT(DISTINCT SaleDetail.ItemId) AS NoOfItemsSold " +
                                            ", COUNT(DISTINCT SaleMaster.Id) AS NoOfSales " +
                                            " FROM SaleMaster INNER JOIN SaleDetail ON SaleMaster.Id = SaleDetail.SaleMasterId " +
                                            " WHERE SaleMaster.SalesDateTime BETWEEN datetime('{2}') AND datetime('{3}') LIMIT 1; "
                        , date.ToString("yyyy-MM-dd") + " 00:00:00"
                        , date.ToString("yyyy-MM-dd") + " 23:59:59"
                        , date.ToString("yyyy-MM-dd") + " 00:00:00"
                        , date.ToString("yyyy-MM-dd") + " 23:59:59");

                        var reader = contents.ExecuteReader();

                        response.param1 = 0;
                        response.param2 = 0;
                        response.dblParam1 = 0.00;
                        while (reader.Read())
                        {
                            object xx = new object();
                            xx = reader["TotalAmount"];
                            if (reader["TotalAmount"] != System.DBNull.Value)
                            {
                                response.dblParam1 = Convert.ToDouble(reader["TotalAmount"]);
                                response.param1 = Convert.ToInt16(reader["NoOfItemsSold"]);
                                response.param2 = Convert.ToInt16(reader["NoOfSales"]);
                            }
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch(Exception e)
            {
                response.message = "Sales(Select): An error has occured.";
            }

            return response;
        }
        public Response getLastId(string dbName)
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
                        contents.CommandText = string.Format("SELECT * FROM SQLITE_SEQUENCE WHERE name = 'SaleMaster';");
                        var reader = contents.ExecuteReader();

                        response.param1 = 0;
                        while (reader.Read())
                        {
                            response.param1 = Convert.ToInt16(reader["seq"]);
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch (Exception e)
            {
                response.message = "Sales(Select): An error has occured.";
            }

            return response;
        }
    }
}