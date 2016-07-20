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
//using SQLite;

namespace POSV1.Model
{
    class SaleMaster
    {
        //[PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public DateTime DATE { get; set; }
        public double AMOUNT { get; set; }
        //public Response insertData(SaleMaster data, string path)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        //var db = new SQLite.SQLiteAsyncConnection(path);
        //        //db.InsertAsync(data);
        //        //response.status = "SUCCESS";
        //        using (var conn = new SQLite.SQLiteConnection(path))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = string.Format("INSERT INTO SaleMaster VALUES({0}, '{1}', {2})", data.ID, data.DATE.ToString("yyyy-MM-dd HH:mm:ss"), data.AMOUNT);
        //            response.saleList = cmd.ExecuteQuery<SaleMaster>();
        //            response.status = "SUCCESS";
        //        }
        //    }
        //    catch
        //    {
        //        response.message = "Error inserting data";
        //    }
        //    return response;
        //}
        //public Response updateData(SaleMaster data, string path)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        using (var conn = new SQLite.SQLiteConnection(path))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = string.Format("UPDATE SaleMaster SET DATE = '{0}', AMOUNT = {1} WHERE ID = {2}", data.DATE.ToString("yyyy-MM-dd HH:mm:ss"), data.AMOUNT, data.ID);
        //            response.saleList = cmd.ExecuteQuery<SaleMaster>();
        //            response.status = "SUCCESS";
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        response.message = "Error updating data";
        //    }
        //    return response;
        //}
        //public Response deleteData(SaleMaster data, string path)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        using (var conn = new SQLite.SQLiteConnection(path))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = string.Format("DELETE FROM SaleMaster WHERE ID = {0}", data.ID);
        //            response.saleList = cmd.ExecuteQuery<SaleMaster>();
        //            response.status = "SUCCESS";
        //        }
        //        response.status = "SUCCESS";
        //    }
        //    catch
        //    {
        //        response.message = "Error deleting data";
        //    }
        //    return response;
        //}
        //public Response findData(int id, string path)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        using (var conn = new SQLite.SQLiteConnection(path))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = "select * from SaleMaster where ID = " + id;
        //            response.saleList = cmd.ExecuteQuery<SaleMaster>();
        //            response.status = "SUCCESS";
        //        }
        //    }
        //    catch
        //    {
        //        response.message = "Error retrieving data";
        //    }
        //    return response;
        //}
        //public Response getData(string dbPath)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        using (var conn = new SQLite.SQLiteConnection(dbPath))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = "select * from SaleMaster";
        //            response.saleList = cmd.ExecuteQuery<SaleMaster>();
        //            response.status = "SUCCESS";
        //        }
        //    }
        //    catch
        //    {
        //        response.message = "Error retrieving data";
        //    }
        //    return response;
        //}
        //public Response getDetail(int id, string dbPath)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        using (var conn = new SQLite.SQLiteConnection(dbPath))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = string.Format("SELECT * FROM SaleDetail WHERE SALEID = {0}", id);
        //            response.saleDetailList = cmd.ExecuteQuery<SaleDetail>();
        //            response.status = "SUCCESS";
        //        }
        //    }
        //    catch
        //    {
        //        response.message = "Error retrieving data";
        //    }
        //    return response;
        //}
        //public Response getId(string dbPath)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        using (var conn = new SQLite.SQLiteConnection(dbPath))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = "SELECT * FROM SaleMaster ORDER BY ID DESC LIMIT 1";
        //            response.saleList = cmd.ExecuteQuery<SaleMaster>();
        //            response.status = "SUCCESS";
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        response.message = "Error generating sales ID.";
        //    }
        //    return response;
        //}
        //public Response getDetailWithItem(int id, string dbPath)
        //{
        //    Response response = new Response();
        //    response.status = "FAILURE";
        //    try
        //    {
        //        using (var conn = new SQLite.SQLiteConnection(dbPath))
        //        {
        //            var cmd = new SQLite.SQLiteCommand(conn);
        //            cmd.CommandText = string.Format("SELECT SaleDetail.*, Item.NAME, Item.RETAILPRICE FROM SaleDetail INNER JOIN Item ON SaleDetail.ITEMID = Item.ID  WHERE SaleDetail.SALEID = {0}", id);
        //            response.saleDetailWithItemList = cmd.ExecuteQuery<SaleDetailWithItem>();
        //            response.status = "SUCCESS";
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        response.message = "Error retrieving data";
        //    }
        //    return response;
        //}
    }
}