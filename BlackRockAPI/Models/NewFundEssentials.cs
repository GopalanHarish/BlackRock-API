using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackRockAPI.Models
{
    public class NewFundEssentials
    {
        public Nullable<long> Id { get; set; }
        public string Title { get; set; }
        public string Identifier { get; set; }
    }
}