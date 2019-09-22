using com.fss.plugin;
using Knet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Knet.Funcion
{
    public class Decrypt
    {
        #region Configuration 
        private static string _resourcePath = WebConfigurationManager.AppSettings["ResourcePath"];
        private static string _keystorePath = WebConfigurationManager.AppSettings["KeystorePath"];
        private static string _alias = WebConfigurationManager.AppSettings["Alias"];
        #endregion 
        //public static int DecryptResponse(string trandata)
        //{
        //    iPayPipe pipe = new iPayPipe();

        //    pipe.setAlias(_alias);
        //    pipe.setResourcePath(_resourcePath);
        //    pipe.setKeystorePath(_keystorePath);
        //    int result = pipe.parseEncryptedRequest(trandata);
        //    return result;
        //}

        public static KnetResponse KnetResponseData(string trandata, string paymentid, string trackid,
           string udf1,
           string udf2,
           string udf3,
           string udf4,
           string udf5
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

            int knetResult = pipe.parseEncryptedRequest(trandata);


            if (knetResult != 0)
            {
                pipe.getError();
                obj.error = pipe.getError();
            }
            else
            {
                obj.paymentId = paymentid;
                obj.trackId = trackid;
                obj.udf1 = udf1;
                obj.udf2 = udf2;
                obj.udf3 = udf3;
                obj.udf4 = udf4;
                obj.udf5 = udf5;
                obj.status = true;
            }
            return obj;
        }
    }
}