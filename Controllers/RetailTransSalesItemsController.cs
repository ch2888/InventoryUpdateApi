using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryUpdateApi.Models;
using System.Net.Http;
using System.Net;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace InventoryUpdateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetailTransSalesItemsController : ControllerBase
    {
        private readonly RetailTransSalesContext _context;

        public RetailTransSalesItemsController(RetailTransSalesContext context)
        {
            _context = context;
        }

        /*
        [HttpGet]
        public HttpResponseMessage GetEmployees(string gender = "All")
        {
            switch (gender.ToLower())
            {
                case "all":
                    return Request.CreateResponse(contentEntities.Employees.ToList());
                case "male":
                    return Request.CreateResponse(contentEntities.Employees.Where(e => e.Gender == "male").ToList());
                case "female":
                    return Request.CreateResponse(contentEntities.Employees.Where(e => e.Gender == "female").ToList());
                default:
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Value for gender must be Male,Female or All." + gender +
                                                       " is invalid.");
            }
        }
        */

        // GET: api/ONHANDINVENTORY
        [HttpGet("{dateOpt}")]
        [Route("OnHandInventory/{dateOpt}")]
        public async Task<ActionResult<IEnumerable<RETAILTRANSACTIONSALESTRANS>>> GetOnHandInventory(string dateOpt)
        {
            //https://localhost:44382/api/RetailTransSalesItems/onhandinventory/5
            var retItems = await _context.RetailTransSalesItems.Where(e => e.store == dateOpt).ToListAsync();
            if (retItems == null)
            {
                return NotFound();
            }
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT      T3.ITEMID as ItemID,T7.NAME as NAME,T5.ITEMGROUPID as ITEMGROUPID,T6.NAME as ITEMGROUPIDNAME,T1.POSTEDQTY as QTY,T4.UNITID as UNITID,T2.INVENTSITEID as INVENTSITEID ,             " +
                    "T2.INVENTLOCATIONID INVENTLOCATIONID, T2.WMSLOCATIONID as WMSLOCATIONID,T2.INVENTBATCHID as INVENTBATCHID,T2.INVENTSERIALID as INVENTSERIALID, T2.WMSPALLETID as WMSPALLETID " +
                    "FROM INVENTSUM T1 " +
                    "INNER JOIN INVENTDIM T2 ON T1.INVENTDIMID = T2.INVENTDIMID " +
                    "INNER JOIN INVENTTABLE T3 ON T3.ITEMID = T1.ITEMID " +
                    "INNER JOIN INVENTTABLEMODULE T4 ON T1.ITEMID = T4.ITEMID AND T4.MODULETYPE = 0 " +
                    "INNER JOIN INVENTITEMGROUPITEM T5 ON T5.ITEMID = T1.ITEMID " +
                    "INNER JOIN INVENTITEMGROUP T6 ON T6.ITEMGROUPID = T5.ITEMGROUPID " +
                    "INNER JOIN ECORESPRODUCTTRANSLATION T7 ON T3.PRODUCT = T7.PRODUCT " +
                    "WHERE T1.DATAAREAID = 'reef'" +
                    " AND T2.DATAAREAID = 'reef'" +
                    " AND T3.DATAAREAID = 'reef'" +
                    " AND T4.DATAAREAID = 'reef'" +
                    " AND T5.ITEMDATAAREAID = 'reef'" +
                    " AND T6.DATAAREAID = 'reef' " +
                    "and t1.POSTEDQTY <> 0 " +
                    //                    "AND T1.ITEMID = CASE WHEN((@ITEMID IS NOT NULL) AND(@ITEMID <> '')) THEN @ITEMID ELSE T1.ITEMID END " +
                    //                  "AND T6.ITEMGROUPID = CASE WHEN((@ITEMGROUPID IS NOT NULL) AND(@ITEMGROUPID <> '')) THEN @ITEMGROUPID ELSE T6.ITEMGROUPID END " +
                    "and T2.INVENTSITEID NOT IN('manufacrtr ', 'HO', 'Moghtabar', 'purchase', 'R-Ex-sales') " +
                    "group by T3.ITEMID , T2.INVENTBATCHID ,T7.NAME,T5.ITEMGROUPID,T6.NAME,T1.POSTEDQTY,T4.UNITID,T2.INVENTSITEID,T2.INVENTLOCATIONID  ,T2.WMSLOCATIONID ,T2.INVENTBATCHID ,T2.INVENTSERIALID , T2.WMSPALLETID";

                command.CommandText = "SELECT top 3 * From RETAILTRANSACTIONSALESTRANS ";

                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    // do something with result
                    Console.WriteLine("result:" + result.ToString());
                    using (DataTable retTable = new DataTable())
                    {
                        retTable.Load(result);

                        foreach (DataRow row in retTable.Rows)
                        {
                            Console.WriteLine("row:" + row.ToString());
                        }
                    }
                }
            }
            return retItems;
        }

        // GET: api/OnInventSum
        //[HttpGet("{dateOpt}")]
        //[Route("OnInventSum/{dateOpt}")]
        //public async Task<ActionResult<IEnumerable<InventSum>>> GetOnInventSum(string dateOpt)
        [HttpGet]
        [Route("OnInventSumv1")]
        public async Task<ActionResult<IEnumerable<InventSumV1>>> GetOnInventSumv1()
        {
            //https://localhost:44382/api/RetailTransSalesItems/OnInventSumv1
            List<InventSumV1> retItems = new List<InventSumV1>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT  TOP (50)    T3.ITEMID as ItemID,T7.NAME as NAME,T5.ITEMGROUPID as ITEMGROUPID,T6.NAME as ITEMGROUPIDNAME,T1.POSTEDQTY as QTY,T4.UNITID as UNITID,T2.INVENTSITEID as INVENTSITEID ,             " +
                    "T2.INVENTLOCATIONID INVENTLOCATIONID, T2.WMSLOCATIONID as WMSLOCATIONID,T2.INVENTBATCHID as INVENTBATCHID,T2.INVENTSERIALID as INVENTSERIALID, T2.WMSPALLETID as WMSPALLETID " +
                    "FROM INVENTSUM T1 " +
                    "INNER JOIN INVENTDIM T2 ON T1.INVENTDIMID = T2.INVENTDIMID " +
                    "INNER JOIN INVENTTABLE T3 ON T3.ITEMID = T1.ITEMID " +
                    "INNER JOIN INVENTTABLEMODULE T4 ON T1.ITEMID = T4.ITEMID AND T4.MODULETYPE = 0 " +
                    "INNER JOIN INVENTITEMGROUPITEM T5 ON T5.ITEMID = T1.ITEMID " +
                    "INNER JOIN INVENTITEMGROUP T6 ON T6.ITEMGROUPID = T5.ITEMGROUPID " +
                    "INNER JOIN ECORESPRODUCTTRANSLATION T7 ON T3.PRODUCT = T7.PRODUCT " +
                    "WHERE T1.DATAAREAID = 'reef'" +
                    " AND T2.DATAAREAID = 'reef'" +
                    " AND T3.DATAAREAID = 'reef'" +
                    " AND T4.DATAAREAID = 'reef'" +
                    " AND T5.ITEMDATAAREAID = 'reef'" +
                    " AND T6.DATAAREAID = 'reef' " +
                    "and t1.POSTEDQTY <> 0 " +
                    //                    "AND T1.ITEMID = CASE WHEN((@ITEMID IS NOT NULL) AND(@ITEMID <> '')) THEN @ITEMID ELSE T1.ITEMID END " +
                    //                  "AND T6.ITEMGROUPID = CASE WHEN((@ITEMGROUPID IS NOT NULL) AND(@ITEMGROUPID <> '')) THEN @ITEMGROUPID ELSE T6.ITEMGROUPID END " +
                    "and T2.INVENTSITEID NOT IN('manufacrtr ', 'HO', 'Moghtabar', 'purchase', 'R-Ex-sales') " +
                    "group by T3.ITEMID , T2.INVENTBATCHID ,T7.NAME,T5.ITEMGROUPID,T6.NAME,T1.POSTEDQTY,T4.UNITID,T2.INVENTSITEID,T2.INVENTLOCATIONID  ,T2.WMSLOCATIONID ,T2.INVENTBATCHID ,T2.INVENTSERIALID , T2.WMSPALLETID";

                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    // do something with result
                    Console.WriteLine("result:" + result.ToString());
                    using (DataTable retTable = new DataTable())
                    {
                        retTable.Load(result);

                        foreach (DataRow row in retTable.Rows)
                        {
                            InventSumV1 r1 = new InventSumV1();
                            Console.WriteLine("row:" + row.ToString());
                            r1.ItemID = row.ItemArray[0].ToString();
                            r1.Name = row.ItemArray[1].ToString();
                            r1.ItemGroupID = row.ItemArray[2].ToString();
                            r1.ItemGroupName = row.ItemArray[3].ToString();
                            r1.Qty = Decimal.Parse(row.ItemArray[4].ToString());
                            r1.UnitID = row.ItemArray[5].ToString();
                            r1.InventSiteID = row.ItemArray[6].ToString();
                            r1.InventLocationID = row.ItemArray[7].ToString();
                            r1.WMSLocationID = row.ItemArray[8].ToString();
                            r1.InventBatchID = row.ItemArray[9].ToString();
                            r1.InventSerialID = row.ItemArray[10].ToString();
                            r1.WMSPalletID = row.ItemArray[11].ToString();
                            retItems.Add(r1);
                        }
                    }
                }
            }

            foreach (InventSumV1 itemOminful in retItems)
            {
                Insert_Ominful_v2();
                break;
            }
            return retItems;
        }

        [HttpGet]
        [Route("OnInventSumv2")]
        public async Task<ActionResult<IEnumerable<InventSumV2>>> GetOnInventSumv2()
        {
            //https://localhost:44382/api/RetailTransSalesItems/OnInventSumv2
            List<InventSumV2> retItems = new List<InventSumV2>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT      T3.ITEMID as ItemID,T7.NAME as NAME,T1.POSTEDQTY as QTY,T2.INVENTSITEID as INVENTSITEID ,t8.storenumber  FROM INVENTSUM T1 INNER JOIN INVENTDIM T2 ON T1.INVENTDIMID = T2.INVENTDIMID INNER JOIN INVENTTABLE T3 ON T3.ITEMID = T1.ITEMID left outer join retailchanneltable t8 on t8.inventlocation = T2.INVENTLOCATIONID INNER JOIN ECORESPRODUCTTRANSLATION T7 ON T3.PRODUCT = T7.PRODUCT WHERE T1.DATAAREAID = 'reef' AND T2.DATAAREAID = 'reef' AND T3.DATAAREAID = 'reef' and t1.POSTEDQTY <>0 and T2.INVENTSITEID  NOT IN (   'manufacrtr ' , 'HO'  , 'Moghtabar' , 'purchase' , 'R-Ex-sales' ) group by T3.ITEMID  ,T7.NAME,T1.POSTEDQTY , T2.INVENTSITEID,t8.storenumber";

                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    // do something with result
                    Console.WriteLine("result:" + result.ToString());
                    using (DataTable retTable = new DataTable())
                    {
                        retTable.Load(result);

                        foreach (DataRow row in retTable.Rows)
                        {
                            InventSumV2 r1 = new InventSumV2();
                            Console.WriteLine("row:" + row.ToString());
                            r1.ItemID = row.ItemArray[0].ToString();
                            r1.Name = row.ItemArray[1].ToString();
                            r1.Qty = ((int)Decimal.Parse(row.ItemArray[2].ToString()));
                            r1.InventSiteID = row.ItemArray[3].ToString();
                            r1.storenumber = row.ItemArray[4].ToString();
                            retItems.Add(r1);
                        }
                    }
                }
            }

            foreach (InventSumV2 itemOminful in retItems)
            {
                Insert_Ominful_v2();
                break;
            }
            return retItems;
        }

        public void Insert_Ominful()
        {
            List<InventSumV1> retItems = new List<InventSumV1>();
            var sqlConn = _context.Database.GetDbConnection();
            using (var command = sqlConn.CreateCommand())
            {
                try
                {
                    /*
                    command.CommandText = " INSERT INTO [dbo].[transactionsfromOminful] " +
                        "(CurrencyCode,CustAccount,CustGroup,LineNum,SalesId,ShippingDateRequested,InventSiteId," +
                        "ItemId,SalesQty,SalesPrice,InventLocationId,LineAmount,DeliveryName,DeliveryStreet,TaxGroup,TaxItemGroup,LineDisc) " +
                        "VALUES(@CurrencyCode, @CustAccount, @CustGroup, @LineNum, @SalesId, @ShippingDateRequested, @InventSiteId, @ItemId, @SalesQty," +
                        " @SalesPrice, @InventLocationId, @LineAmount, @DeliveryName, @DeliveryStreet, @TaxGroup, @TaxItemGroup, @LineDisc); ";
                    */
                    command.CommandText = " INSERT INTO [dbo].[transactionsfromOminful] (CurrencyCode,CustAccount,CustGroup,LineNum,SalesId," +
                        "ShippingDateRequested,InventSiteId,ItemId,SalesQty,SalesPrice,InventLocationId,LineAmount,DeliveryName,DeliveryStreet,TaxGroup,TaxItemGroup,LineDisc)" +
                        " VALUES('SAR', 'Reef-000014', 'Retail', 1, 8, '2022-03-26', 'Al Samer', '100001', 4,500, '021-Samer', 200, 'Mohamed', 'Riyadh', 'vat Group', 'Vat Items', 50); ";

                    _context.Database.OpenConnection();
                    command.CreateParameter();
                    int nRows = command.ExecuteNonQuery();
                    Console.WriteLine("Inserted {0} rows.", nRows);

                }
                catch (DbException exDb)
                {
                    Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                    Console.WriteLine("DbException.Source: {0}", exDb.Source);
                    Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                    Console.WriteLine("DbException.Message: {0}", exDb.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }

            }
            /*

            System.Data.SqlClient.SqlConnection sqlConn1 = new System.Data.SqlClient.SqlConnection();
            //sqlConn.ConnectionString = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnectionString;
            string query = "INSERT INTO ax.Retailworkerdiscounttrans (TransactionId, workerid, phonenumber, discountamount, storeid, receiptnumber, TRANSDATATIME,discpercentage)";
            query += " VALUES (@transactionId, @workerid,@phonenumber, @discountamount, @storeid, @receiptnumber, @TRANSDATATIME,@discpercentage)";

            SqlDataAdapter da = new SqlDataAdapter();
            da.InsertCommand = new SqlCommand(query, sqlConn1);



            da.InsertCommand.Parameters.Add("@transactionId", SqlDbType.VarChar).Value = strTransId;
            da.InsertCommand.Parameters.Add("@workerid", SqlDbType.VarChar).Value = strWorkerid;
            da.InsertCommand.Parameters.Add("@phonenumber", SqlDbType.Decimal).Value = strPN;
            da.InsertCommand.Parameters.Add("@discountamount", SqlDbType.VarChar).Value = strDiscountAmount;
            da.InsertCommand.Parameters.Add("@storeid", SqlDbType.VarChar).Value = strStoreId;
            da.InsertCommand.Parameters.Add("@receiptnumber", SqlDbType.VarChar).Value = strReceiptNum;
            da.InsertCommand.Parameters.Add("@TRANSDATATIME", SqlDbType.DateTime).Value = DateTime.Now;
            da.InsertCommand.Parameters.Add("@discpercentage", SqlDbType.VarChar).Value = fDiscountPercentage.ToString();


            sqlConn.Open();
            da.InsertCommand.ExecuteNonQuery();
            sqlConn.Close();
            */
        }

        public void Insert_Ominful_v2()
        {
            List<InventSumV2> retItems = new List<InventSumV2>();
            var sqlConn = _context.Database.GetDbConnection();
            using (var command = sqlConn.CreateCommand())
            {
                try
                {
                    /*
                    command.CommandText = " INSERT INTO [dbo].[transactionsfromOminful] " +
                        "(CurrencyCode,CustAccount,CustGroup,LineNum,SalesId,ShippingDateRequested,InventSiteId," +
                        "ItemId,SalesQty,SalesPrice,InventLocationId,LineAmount,DeliveryName,DeliveryStreet,TaxGroup,TaxItemGroup,LineDisc) " +
                        "VALUES(@CurrencyCode, @CustAccount, @CustGroup, @LineNum, @SalesId, @ShippingDateRequested, @InventSiteId, @ItemId, @SalesQty," +
                        " @SalesPrice, @InventLocationId, @LineAmount, @DeliveryName, @DeliveryStreet, @TaxGroup, @TaxItemGroup, @LineDisc); ";
                    */
                    command.CommandText = " declare @ShippingDateRequested as  datetime; declare @SalesId as  nvarchar(50);" +
                        "set @ShippingDateRequested = getdate();" +
                        "set @SalesId = ( select max ( salesid )from [dbo].[transactionsfromOminful]  )  +1;" +
                        " INSERT INTO [dbo].[transactionsfromOminful] (CurrencyCode,CustAccount,CustGroup,LineNum,SalesId," +
                        "ShippingDateRequested,InventSiteId,ItemId,SalesQty,SalesPrice,InventLocationId,LineAmount,DeliveryName,DeliveryStreet,TaxGroup,TaxItemGroup,LineDisc)" +
                        " VALUES('SAR', 'Reef-000014', 'Retail', 1, @SalesId, @ShippingDateRequested, 'Al Samer', '100001', 4,500, '021-Samer', 200, 'Mohamed', 'Riyadh', 'vat Group', 'Vat Items', 50); ";

                    _context.Database.OpenConnection();
                    command.CreateParameter();
                    int nRows = command.ExecuteNonQuery();
                    Console.WriteLine("Inserted {0} rows.", nRows);

                }
                catch (DbException exDb)
                {
                    Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                    Console.WriteLine("DbException.Source: {0}", exDb.Source);
                    Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                    Console.WriteLine("DbException.Message: {0}", exDb.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }

            }
            /*

            System.Data.SqlClient.SqlConnection sqlConn1 = new System.Data.SqlClient.SqlConnection();
            //sqlConn.ConnectionString = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnectionString;
            string query = "INSERT INTO ax.Retailworkerdiscounttrans (TransactionId, workerid, phonenumber, discountamount, storeid, receiptnumber, TRANSDATATIME,discpercentage)";
            query += " VALUES (@transactionId, @workerid,@phonenumber, @discountamount, @storeid, @receiptnumber, @TRANSDATATIME,@discpercentage)";

            SqlDataAdapter da = new SqlDataAdapter();
            da.InsertCommand = new SqlCommand(query, sqlConn1);



            da.InsertCommand.Parameters.Add("@transactionId", SqlDbType.VarChar).Value = strTransId;
            da.InsertCommand.Parameters.Add("@workerid", SqlDbType.VarChar).Value = strWorkerid;
            da.InsertCommand.Parameters.Add("@phonenumber", SqlDbType.Decimal).Value = strPN;
            da.InsertCommand.Parameters.Add("@discountamount", SqlDbType.VarChar).Value = strDiscountAmount;
            da.InsertCommand.Parameters.Add("@storeid", SqlDbType.VarChar).Value = strStoreId;
            da.InsertCommand.Parameters.Add("@receiptnumber", SqlDbType.VarChar).Value = strReceiptNum;
            da.InsertCommand.Parameters.Add("@TRANSDATATIME", SqlDbType.DateTime).Value = DateTime.Now;
            da.InsertCommand.Parameters.Add("@discpercentage", SqlDbType.VarChar).Value = fDiscountPercentage.ToString();


            sqlConn.Open();
            da.InsertCommand.ExecuteNonQuery();
            sqlConn.Close();
            */
        }

        [HttpGet("{LineNum}/{InventSiteId}/{ItemId}/{SalesQty}/{SalesPrice}/{InventLocationId}/{LineAmount}/{DeliveryName}/{DeliveryStreet}/{LineDisc}")]
        [Route("OnInsertOminful/{LineNum}/{InventSiteId}/{ItemId}/{SalesQty}/{SalesPrice}/{InventLocationId}/{LineAmount}/{DeliveryName}/{DeliveryStreet}/{LineDisc}")]
        public string GetOnInsertOminful(
            string LineNum,
            string InventSiteId,
            string ItemId,
            string SalesQty,
            string SalesPrice,
            string InventLocationId,
            string LineAmount,
            string DeliveryName,
            string DeliveryStreet,
            string LineDisc
            )
        {
            //https://localhost:44382/api/RetailTransSalesItems/OnInsertOminful/1/AlSamer/100001/4/500/021-Samer/200/Mohamed/Riyadh/50
            string ret = "Success";
            // check param validation.
            if (
                LineNum.Replace(" ", "").Length == 0 ||
                InventSiteId.Replace(" ", "").Length == 0 ||
                ItemId.Replace(" ", "").Length == 0 ||
                SalesQty.Replace(" ", "").Length == 0 ||
                SalesPrice.Replace(" ", "").Length == 0 ||
                InventLocationId.Replace(" ", "").Length == 0 ||
                LineAmount.Replace(" ", "").Length == 0 ||
                DeliveryName.Replace(" ", "").Length == 0 ||
                DeliveryStreet.Replace(" ", "").Length == 0 ||
                LineDisc.Replace(" ", "").Length == 0
                )
            {
                return "Invalid Param";
            }
            // ominful v3
            List<InventSumV2> retItems = new List<InventSumV2>();
            var sqlConn = _context.Database.GetDbConnection();
            using (var command = sqlConn.CreateCommand())
            {
                try
                {
                    command.CommandText = " declare @ShippingDateRequested as  datetime; declare @SalesId as  nvarchar(50);" +
                        "set @ShippingDateRequested = getdate();" +
                        "set @SalesId = ( select max ( salesid ) + 1 from [dbo].[transactionsfromOminful]  )  ;" +
                        " INSERT INTO [dbo].[transactionsfromOminful] (CurrencyCode,CustAccount,CustGroup,LineNum,SalesId," +
                        "ShippingDateRequested,InventSiteId,ItemId,SalesQty,SalesPrice,InventLocationId,LineAmount,DeliveryName,DeliveryStreet,TaxGroup,TaxItemGroup,LineDisc)" +
                        " VALUES('SAR', 'Reef-000014', 'Retail', " +
                        LineNum +
                        ", @SalesId, @ShippingDateRequested, '" +
                        InventSiteId +
                        "', '" +
                        ItemId + "'," +
                        SalesQty + "," +
                        SalesPrice + ", '" +
                        InventLocationId + "', " +
                        LineAmount + ", '" +
                        DeliveryName + "', '" +
                        DeliveryStreet + "', 'vat Group', 'Vat Items', " +
                        LineDisc + "); ";

                    _context.Database.OpenConnection();
                    command.CreateParameter();
                    int nRows = command.ExecuteNonQuery();
                    Console.WriteLine("Inserted {0} rows.", nRows);

                }
                catch (DbException exDb)
                {
                    Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                    Console.WriteLine("DbException.Source: {0}", exDb.Source);
                    Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                    Console.WriteLine("DbException.Message: {0}", exDb.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }

            }
            return ret;
        }

        public void Insert_Ominful_v3()
        {
            List<InventSumV2> retItems = new List<InventSumV2>();
            var sqlConn = _context.Database.GetDbConnection();
            using (var command = sqlConn.CreateCommand())
            {
                try
                {
                    command.CommandText = " declare @ShippingDateRequested as  datetime; declare @SalesId as  nvarchar(50);" +
                        "set @ShippingDateRequested = getdate();" +
                        "set @SalesId = ( select max ( salesid )from [dbo].[transactionsfromOminful]  )  +1;" +
                        " INSERT INTO [dbo].[transactionsfromOminful] (CurrencyCode,CustAccount,CustGroup,LineNum,SalesId," +
                        "ShippingDateRequested,InventSiteId,ItemId,SalesQty,SalesPrice,InventLocationId,LineAmount,DeliveryName,DeliveryStreet,TaxGroup,TaxItemGroup,LineDisc)" +
                        " VALUES('SAR', 'Reef-000014', 'Retail', 1, @SalesId, @ShippingDateRequested, 'Al Samer', '100001', 4,500, '021-Samer', 200, 'Mohamed', 'Riyadh', 'vat Group', 'Vat Items', 50); ";

                    _context.Database.OpenConnection();
                    command.CreateParameter();
                    int nRows = command.ExecuteNonQuery();
                    Console.WriteLine("Inserted {0} rows.", nRows);

                }
                catch (DbException exDb)
                {
                    Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                    Console.WriteLine("DbException.Source: {0}", exDb.Source);
                    Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                    Console.WriteLine("DbException.Message: {0}", exDb.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }

            }
        }

        // GET: api/OnInvent
        [HttpGet("{dateOpt}")]
        [Route("OnInvent/{dateOpt}")]
        public async Task<ActionResult<IEnumerable<InventRet>>> GetOnInvent(string dateOpt)
        {
            //https://localhost:44382/api/RetailTransSalesItems/OnInvent/5
            List<InventRet> retItems = new List<InventRet>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT top 3 * From RETAILTRANSACTIONSALESTRANS ";

                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    // do something with result
                    Console.WriteLine("result:" + result.ToString());
                    using (DataTable retTable = new DataTable())
                    {
                        retTable.Load(result);

                        foreach (DataRow row in retTable.Rows)
                        {
                            InventRet r1 = new InventRet();
                            Console.WriteLine("row:" + row.ToString());
                            r1.receiptid = row.ItemArray[3].ToString();
                            r1.qty = Decimal.Parse(row.ItemArray[8].ToString());
                            //r1.transdate = row.ItemArray[13];
                            r1.store = row.ItemArray[29].ToString();
                            retItems.Add(r1);
                        }
                    }
                }
            }
            return retItems;
        }

        // GET: api/RetailTransSalesItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RETAILTRANSACTIONSALESTRANS>>> GetRetailTransSalesItems()
        {
            return await _context.RetailTransSalesItems.ToListAsync();
        }
        /*
        // GET: api/RetailTransSalesItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RETAILTRANSACTIONSALESTRANS>> GetRetailTransSalesItem(long id)
        {
            var retailTransSalesItem = await _context.RetailTransSalesItems.FindAsync(id);

            if (retailTransSalesItem == null)
            {
                return NotFound();
            }

            return retailTransSalesItem;
        }

        // PUT: api/RetailTransSalesItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRetailTransSalesItem(long id, RETAILTRANSACTIONSALESTRANS retailTransSalesItem)
        {
            if (id != retailTransSalesItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(retailTransSalesItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RetailTransSalesItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RetailTransSalesItems
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RETAILTRANSACTIONSALESTRANS>> PostRetailTransSalesItem(RETAILTRANSACTIONSALESTRANS retailTransSalesItem)
        {
            _context.RetailTransSalesItems.Add(retailTransSalesItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRetailTransSalesItem", new { id = retailTransSalesItem.Id }, retailTransSalesItem);
        }

        // DELETE: api/RetailTransSalesItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RETAILTRANSACTIONSALESTRANS>> DeleteRetailTransSalesItem(long id)
        {
            var retailTransSalesItem = await _context.RetailTransSalesItems.FindAsync(id);
            if (retailTransSalesItem == null)
            {
                return NotFound();
            }

            _context.RetailTransSalesItems.Remove(retailTransSalesItem);
            await _context.SaveChangesAsync();

            return retailTransSalesItem;
        }

        private bool RetailTransSalesItemExists(long id)
        {
            return _context.RetailTransSalesItems.Any(e => e.Id == id);
        }
        */
    }
}
