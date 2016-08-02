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

namespace POSV1.Model
{
    class SalesSummary
    {
        public int SaleMasterId { get; set; }
        public int SaleDetailId { get; set; }
        public DateTime SalesDateTime { get; set; }
        public double Amount { get; set; }
        public double AmountReceived { get; set; }
        public int ItemsSold { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string SoldBy { get; set; }
        public string SoldTo { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public double PurchasedPrice { get; set; }
        public double RetailPrice { get; set; }
        public int AvailableStock { get; set; }
        public double AverageCost { get; set; }
        public Response getSalesByDateTime(DateTime date, string dbName, int currentItems)
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
                        contents.CommandText = string.Format("SELECT SaleMaster.Id AS SaleMasterId, SaleMaster.SalesDateTime AS SalesDateTime,  SaleMaster.Amount AS Amount, (SELECT COUNT(ItemId) FROM SaleDetail WHERE SaleMasterId = SaleMaster.Id) AS ItemsSold, SaleMaster.SoldBy AS SoldBy, SaleMaster.SoldTo AS SoldTo FROM SaleMaster WHERE SaleMaster.SalesDateTime BETWEEN '{0}' AND '{1}' ORDER BY SaleMaster.SalesDateTime DESC LIMIT {2}, {3};"
                            , date.ToString("yyyy-MM-dd") + " 00:00:00"
                            , date.ToString("yyyy-MM-dd") + " 23:59:59"
                            , currentItems //No of records to skip
                            , 20 /*No of records to be retrieved*/);

                        var reader = contents.ExecuteReader();
                        response.salesSummary = new List<SalesSummary>();
                        while (reader.Read())
                        {
                            object xx = new object();
                            xx = reader["Amount"];
                            if (reader["Amount"] != System.DBNull.Value)
                            {
                                response.salesSummary.Add(new SalesSummary()
                                {
                                    SaleMasterId = Convert.ToInt16(reader["SaleMasterId"])
                                    //, SaleDetailId = Convert.ToInt16(reader["SaleDetailId"])
                                    , SalesDateTime = Convert.ToDateTime(reader["SalesDateTime"])
                                    , Amount = Convert.ToDouble(reader["Amount"])
                                    , ItemsSold = Convert.ToInt16(reader["ItemsSold"])
                                    //, ItemId = Convert.ToInt16(reader["ItemId"])
                                    //, Quantity = Convert.ToInt16(reader["Quantity"])
                                    , SoldBy = reader["SoldBy"].ToString()
                                    , SoldTo = reader["SoldTo"].ToString()
                                });
                            }
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch (Exception e)
            {
                response.message = "Sales Summary(Select): An error has occured.";
            }

            return response;
        }
        public Response getSaleById(int saleMasterId, string dbName)
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
                        contents.CommandText = string.Format("SELECT    SaleMaster.Id AS SaleMasterId" +
		                                        ", SaleMaster.SalesDateTime AS SalesDateTime " +
		                                        ", SaleMaster.Amount AS Amount " +
		                                        ", SaleMaster.AmountReceived AS AmountReceived " +
		                                        ", SaleMaster.SoldBy AS SoldBy " +
		                                        ", SaleMaster.SoldTo AS SoldTo " +
		                                        ", SaleDetail.Id AS SaleDetailId " +
		                                        ", SaleDetail.Quantity AS Quantity " +
		                                        ", Item.Id AS ItemId " +
		                                        ", Item.Barcode AS Barcode " +
		                                        ", Item.NAME AS ItemName " +
		                                        ", Item.PurchasedPrice AS PurchasedPrice " +
		                                        ", Item.RetailPrice AS RetailPrice " +
		                                        ", Item.AvailableStock AS AvailableStock " +
		                                        ", Item.AverageCost AS AverageCost " +
                                        "FROM SaleMaster INNER JOIN SaleDetail ON SaleMaster.Id = SaleDetail.SaleMasterId " +
                                        "                LEFT JOIN Item ON SaleDetail.ItemId = Item.Id " +
                                        "WHERE SaleMaster.Id = {0};", saleMasterId);

                        var reader = contents.ExecuteReader();
                        response.salesSummary = new List<SalesSummary>();
                        while (reader.Read())
                        {
                            object xx = new object();
                            xx = reader["Amount"];
                            if (reader["Amount"] != System.DBNull.Value)
                            {
                                response.salesSummary.Add(new SalesSummary()
                                {
                                    SaleMasterId = Convert.ToInt16(reader["SaleMasterId"])
                                    , SaleDetailId = Convert.ToInt16(reader["SaleDetailId"])
                                    , SalesDateTime = Convert.ToDateTime(reader["SalesDateTime"])
                                    , Amount = Convert.ToDouble(reader["Amount"])
                                    , AmountReceived = Convert.ToDouble(reader["AmountReceived"])
                                    //, ItemsSold = Convert.ToInt16(reader["ItemsSold"])
                                    , ItemId = Convert.ToInt16(reader["ItemId"])
                                    , Quantity = Convert.ToInt16(reader["Quantity"])
                                    , SoldBy = reader["SoldBy"].ToString()
                                    , SoldTo = reader["SoldTo"].ToString()
                                    , Barcode = reader["Barcode"].ToString()
                                    , ItemName = reader["ItemName"].ToString()
                                    , PurchasedPrice = Convert.ToDouble(reader["PurchasedPrice"])
                                    , RetailPrice = Convert.ToDouble(reader["RetailPrice"])
                                    , AvailableStock = Convert.ToInt16(reader["AvailableStock"])
                                    , AverageCost = Convert.ToDouble(reader["AverageCost"])
                                });
                            }
                        }
                    }
                    conn.Close();
                }
                response.status = "SUCCESS";
            }
            catch (Exception e)
            {
                response.message = "Sale Summary(Select): An error has occured.";
            }

            return response;
        }
    }
}