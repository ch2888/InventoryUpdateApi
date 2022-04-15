using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryUpdateApi.Models
{
    public class RETAILTRANSACTIONSALESTRANS
    {
        //public long Id { get; set; }
        public string receiptid { get; set; }
        public string store { get; set; }
        public string transdate { get; set; }
        public int qty { get; set; }

    }


    public class InventRet
    {
        //        public long Id { get; set; }
        public string receiptid { get; set; }
        public string store { get; set; }
        public DateTime transdate { get; set; }
        public decimal qty { get; set; }

    }

    public class InventSumV1
    {
        //        public long Id { get; set; }
        public string ItemID { get; set; }
        public string Name { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }

        public decimal Qty { get; set; }
        public string UnitID { get; set; }
        public string InventSiteID { get; set; }
        public string InventLocationID { get; set; }
        public string WMSLocationID { get; set; }
        public string InventBatchID { get; set; }
        public string InventSerialID { get; set; }
        public string WMSPalletID { get; set; }


    }

    public class InventSumV2
    {
        //        public long Id { get; set; }
        public string ItemID { get; set; }
        public string Name { get; set; }
        public decimal Qty { get; set; }
        public string InventSiteID { get; set; }
        public string storenumber { get; set; }


    }

}
