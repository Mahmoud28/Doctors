using com.fss.plugin;
using Knet.Helper.ExceptionLog;
using Knet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace Knet.Controllers
{
    public class KnetController : ApiController
    {
        #region Configuration 
        private string _resourcePath = WebConfigurationManager.AppSettings["ResourcePath"];
        private string _keystorePath = WebConfigurationManager.AppSettings["KeystorePath"];
        private string _responseUrl = WebConfigurationManager.AppSettings["ResponseUrl"];
        private string _errorUrl = WebConfigurationManager.AppSettings["ErrorUrl"];
        private string _alias = WebConfigurationManager.AppSettings["Alias"];
        #endregion 

        // Post api/knet/Obj
        [HttpPost]
        public KnetModel Get([FromBody]KnetModel Obj)
        {
            Obj = MappingFromRequest(Obj);

            try
            {
                new ExceptionLog().BeginKnetRequest(JsonConvert.SerializeObject(Obj));

                iPayPipe MyObj = new iPayPipe();

                MyObj.setKeystorePath(_keystorePath);
                MyObj.setResourcePath(_resourcePath);
                if (string.IsNullOrEmpty(Obj.ResponseURL))
                    MyObj.setResponseURL(_responseUrl + Obj.OrderId);
                else
                    MyObj.setResponseURL(Obj.ResponseURL + Obj.OrderId);

                if (string.IsNullOrEmpty(Obj.ErrorUrl))
                    MyObj.setErrorURL(_errorUrl);
                else
                    MyObj.setErrorURL(Obj.ErrorUrl + Obj.OrderId);

                MyObj.setErrorURL(_errorUrl);
                MyObj.setAlias(_alias);
                MyObj.setAction("1");
                MyObj.setAmt(Obj.Amt);
                MyObj.setCurrency(Obj.Currency);
                MyObj.setLanguage(Obj.Language);
                MyObj.setTrackId(Obj.TrackId);
                MyObj.setPaymentId(Obj.PaymentId);
                //MyObj.setTransId("117601");
                MyObj.setUdf1(Obj.Udf1);
                MyObj.setUdf2(Obj.Udf2);
                MyObj.setUdf3(Obj.Udf3);
                MyObj.setUdf4(Obj.Udf4);
                MyObj.setUdf5(Obj.Udf5);
                //

                //MyObj.setTrackId("1176");
                //MyObj.setTranportalId("117601");
                //MyObj.setTransId("117601");
                //MyObj.setPaymentId(Obj.TrackId);

                // For Hosted Payment Integration, the method to be called
                int val = MyObj.performPaymentInitializationHTTP();

                Obj.transVal = short.Parse(MyObj.performPaymentInitializationHTTP().ToString());

                if (val == 0)
                {
                    //Response.sendRedirect(pipe.getWebAddress()); //To redirect the web address.
                }
                else
                {
                    Obj.ErrorMsg = MyObj.getError();
                }
                /** End of Request Processing**/

                Obj.PaymentId = MyObj.getPaymentId();
                Obj.PaymentPage = MyObj.getPaymentPage();
                //MyObj.getWebAddress
                Obj.ErrorMsg = MyObj.getError();
                Obj.RawResponse = MyObj.getRawResponse();
                Obj.Result = MyObj.getResult();
                //Obj.ErrorUrl = "koki";

                Obj.PaymentPage = MyObj.getWebAddress();
             
                // errorurl
            }
            catch (Exception e)
            {
                new ExceptionLog().ErrorLogging(e, ErrorLogType.KnetPaymentError);
            }
            return Obj;
        }


        private KnetModel MappingFromRequest(KnetModel Obj)
        {
            KnetModel model = new KnetModel();

            //model.Alias = _alias;
            //model.ResourcePath = _resourcePath;
            //model.ResponseURL = _responseUrl;
            //model.ErrorUrl = _errorUrl;

            model.Amt = Obj.Amt;
            model.Action = Obj.Action;
            model.Currency = Obj.Currency;
            model.ErrorMsg = Obj.ErrorMsg;
            model.Language = Obj.Language;
            model.TrackId = Obj.TrackId;
            model.transVal = Obj.transVal;
            model.PaymentId = Obj.PaymentId;
            model.OrderId = Obj.OrderId;
            model.Udf1 = Obj.Udf1;
            model.Udf2 = Obj.Udf2;
            model.Udf3 = Obj.Udf3;
            model.Udf4 = Obj.Udf4;
            model.Udf5 = Obj.Udf5;
            return model;
        }
    }
}
