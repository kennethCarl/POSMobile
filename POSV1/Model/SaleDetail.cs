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
    class SaleDetail
    {
        //[PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int SALEID { get; set; }
        public int ITEMID { get; set; }
        public int QUANTITY { get; set; }

    //    public Response insertData(SaleDetail data, string path)
    //    {
    //        Response response = new Response();
    //        response.status = "FAILURE";
    //        try
    //        {
    //            var db = new SQLite.SQLiteAsyncConnection(path);
    //            db.InsertAsync(data);
    //            response.status = "SUCCESS";
    //        }
    //        catch
    //        {
    //            response.message = "Error inserting data";
    //        }
    //        return response;
    //    }
    //    public Response updateData(SaleDetail data, string path)
    //    {
    //        Response response = new Response();
    //        response.status = "FAILURE";
    //        try
    //        {
    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("UPDATE SaleDetail SET ITEMID = {0}, QUANTITY = {1} WHERE ID = {2} AND SALEID = {3}", data.ITEMID, data.QUANTITY, data.ID, data.SALEID);
    //                response.saleDetailList = cmd.ExecuteQuery<SaleDetail>();
    //                response.status = "SUCCESS";
    //            }
    //        }
    //        catch
    //        {
    //            response.message = "Error updating data";
    //        }
    //        return response;
    //    }
    //    public Response deleteData(SaleDetail data, string path)
    //    {
    //        Response response = new Response();
    //        response.status = "FAILURE";
    //        try
    //        {
    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("DELETE FROM SaleDetail WHERE ID = {0} AND SALEID = {1}", data.ID, data.SALEID);
    //                response.saleDetailList = cmd.ExecuteQuery<SaleDetail>();
    //                response.status = "SUCCESS";
    //            }
    //        }
    //        catch(Exception e)
    //        {
    //            response.message = "Error deleting data";
    //        }
    //        return response;
    //    }
    //    public Response deleteByMasterData(SaleMaster sale, string path)
    //    {
    //        Response response = new Response();
    //        List<SaleDetail> saleDetails = new List<SaleDetail>();
    //        List<Item> item= new List<Item>();
    //        response.status = "FAILURE";
    //        try
    //        {
    //            //Get details
    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("SELECT * FROM SaleDetail WHERE SALEID = {0}", sale.ID);
    //                saleDetails = cmd.ExecuteQuery<SaleDetail>();
    //            }

    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("BEGIN TRANSACTION;");
    //                response.saleList = cmd.ExecuteQuery<SaleMaster>();
    //            }

    //            foreach (SaleDetail saleDetailsModel in saleDetails)
    //            {
    //                //Update Item Available Quantity
    //                using (var conn = new SQLite.SQLiteConnection(path))
    //                {
    //                    var cmd = new SQLite.SQLiteCommand(conn);
    //                    cmd.CommandText = string.Format("UPDATE ITEM SET AVAILABLESTOCK = AVAILABLESTOCK + {0} WHERE ID = {1}", saleDetailsModel.QUANTITY, saleDetailsModel.ITEMID);
    //                    saleDetails = cmd.ExecuteQuery<SaleDetail>();
    //                }
    //            }

    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("DELETE FROM SaleDetail WHERE SALEID = {0}", sale.ID);
    //                response.saleDetailList = cmd.ExecuteQuery<SaleDetail>();
    //                response.status = "SUCCESS";
    //            }

    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("COMMIT;");
    //                response.saleList = cmd.ExecuteQuery<SaleMaster>();
    //            }
    //        }
    //        catch
    //        {
    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("ROLLBACK;");
    //                response.saleList = cmd.ExecuteQuery<SaleMaster>();
    //            }
    //            response.message = "Error deleting data";
    //        }
    //        return response;
    //    }
    //    public Response findData(int id, int saleId, string path)
    //    {
    //        Response response = new Response();
    //        response.status = "FAILURE";
    //        try
    //        {
    //            using (var conn = new SQLite.SQLiteConnection(path))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = string.Format("SELECT * FROM SaleDetail where ID = {0} AND SALEID = {1}", id, saleId);
    //                response.saleDetailList = cmd.ExecuteQuery<SaleDetail>();
    //                response.status = "SUCCESS";
    //            }
    //        }
    //        catch
    //        {
    //            response.message = "Error retrieving data";
    //        }
    //        return response;
    //    }
    //    public Response getData(string dbPath)
    //    {
    //        Response response = new Response();
    //        response.status = "FAILURE";
    //        try
    //        {
    //            using (var conn = new SQLite.SQLiteConnection(dbPath))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = "SELECT * FROM SaleDetail";
    //                response.saleDetailList = cmd.ExecuteQuery<SaleDetail>();
    //                response.status = "SUCCESS";
    //            }
    //        }
    //        catch
    //        {
    //            response.message = "Error retrieving data";
    //        }
    //        return response;
    //    }
    //    public Response getId(string dbPath)
    //    {
    //        Response response = new Response();
    //        response.status = "FAILURE";
    //        try
    //        {
    //            using (var conn = new SQLite.SQLiteConnection(dbPath))
    //            {
    //                var cmd = new SQLite.SQLiteCommand(conn);
    //                cmd.CommandText = "SELECT * FROM SaleDetail ORDER BY ID DESC LIMIT 1";
    //                response.saleDetailList = cmd.ExecuteQuery<SaleDetail>();
    //                response.status = "SUCCESS";
    //            }
    //        }
    //        catch(Exception e)
    //        {
    //            response.message = "Error generating sales ID.";
    //        }
    //        return response;
    //    }
    }
}