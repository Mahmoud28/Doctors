using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Knet.Models
{
    public class KnetModel
    {
        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public string Amt { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public string TrackId { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public string Udf1 { get; set; }

        [DataMember]
        public string Udf2 { get; set; }

        [DataMember]
        public string Udf3 { get; set; }

        [DataMember]
        public string Udf4 { get; set; }

        [DataMember]
        public string Udf5 { get; set; }

        [DataMember]
        public string PaymentId { get; set; }

        [DataMember]
        public string PaymentPage { get; set; }

        [DataMember]
        public string ErrorMsg { get; set; }

        [DataMember]
        public short transVal { get; set; }

        [DataMember]
        public string RawResponse { get; set; }

        [DataMember]
        public string ErrorUrl { get; set; }

        [DataMember]
        public string ResourcePath { get; set; }

        [DataMember]
        public string ResponseURL { get; set; }

        //[DataMember]
        public string Result { get; set; }
        public int OrderId { get; set; }
    }

    public class KnetResponseDecryptModel
    {
        public string trandata { get; set; }
        public string paymentid { get; set; }
        public string trackid { get; set; }
        public string udf1 { get; set; }
        public string udf2 { get; set; }
        public string udf3 { get; set; }
        public string udf4 { get; set; }
        public string udf5 { get; set; }
    }
}