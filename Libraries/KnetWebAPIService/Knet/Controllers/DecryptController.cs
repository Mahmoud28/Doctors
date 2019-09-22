using com.fss.plugin;
using Knet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace Knet.Controllers
{
    public class DecryptController : ApiController
    {
        #region Configuration 
        private string _resourcePath = WebConfigurationManager.AppSettings["ResourcePath"];
        private string _keystorePath = WebConfigurationManager.AppSettings["KeystorePath"];
        private string _responseUrl = WebConfigurationManager.AppSettings["ResponseUrl"];
        private string _errorUrl = WebConfigurationManager.AppSettings["ErrorUrl"];
        private string _alias = WebConfigurationManager.AppSettings["Alias"];
        #endregion 

        // POST: api/Decrypt
        [HttpPost]
        public KnetResponse Decrypt([FromBody] KnetResponseDecryptModel Obj
           )
        {
            KnetResponse obj = new KnetResponse();
            obj.status = false;
            iPayPipe pipe = new iPayPipe();

            string resourcePath = _resourcePath;
            string keystorePath = _keystorePath;
            string aliasName = _alias;

            pipe.setAlias(aliasName);
            pipe.setResourcePath(resourcePath);
            pipe.setKeystorePath(resourcePath);

            int knetResult = pipe.parseEncryptedRequest(Obj.trandata);


            if (knetResult != 0)
            {
                pipe.getError();
                obj.error = pipe.getError();
            }
            else
            {
                obj.paymentId = Obj.paymentid;
                obj.trackId = Obj.trackid;
                obj.udf1 = Obj.udf1;
                obj.udf2 = Obj.udf2;
                obj.udf3 = Obj.udf3;
                obj.udf4 = Obj.udf4;
                obj.udf5 = Obj.udf5;
                obj.result = pipe.getResult();
                obj.status = obj.result == "CAPTURED" ? true :false;
            }
            return obj;
        }

    }
}
