using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackRockAPI.Models
{
    public class FundsUnderManagement
    {
        public string Title { get; set; }
        public System.DateTime ManagingSince { get; set; }
        public decimal AUM { get; set; }
        public decimal ExpenseRatio { get; set; }
    }
}