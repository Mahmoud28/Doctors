using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Knet.Models
{
    public class KnetResponse
    {
        public string paymentId { get; set; }
        public string result { get; set; }
        public string postDate { get; set; }
        public string transId { get; set; }
        public string auth { get; set; }
        public string reference { get; set; }
        public string trackId { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
        public bool status { get; set; }
        public string error { get; set; }
    }
}