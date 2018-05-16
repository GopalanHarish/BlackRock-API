using BlackRockAPI.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackRockAPI.Models
{
    public class AppUser
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public System.DateTime RegisteredOn { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> LastLoggedOn { get; set; }
        public string Bio { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<FundsUnderManagement> Funds { get; set; }
    }
}