using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlackRockAPI.Models
{
    public class Funds
    {
        public long Id { get; set; }
        public string Inception_Date { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Goal { get; set; }
        public string Horizone { get; set; }
        public string Fund_Manager { get; set; }
    }
}