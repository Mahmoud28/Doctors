using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Knet.Helper.ExceptionLog
{
    public class ExceptionLog
    {
        public void WriteException(Exception ex)
        {
            string filePath = @"C:\Error.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
        public void ErrorLogging(Exception ex, string type)
        {
            string strPath = string.Empty;// Directory.GetCurrentDirectory();
            switch (type)
            {
                case (string)ErrorLogType.KnetPaymentError:
                    strPath += HttpContext.Current.Server.MapPath(@"\SystemLogs\KnetPayment");
                    break;
            }
            if (!string.IsNullOrEmpty(strPath))
            {

                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strPath += "/Error_" + DateTime.Now.Ticks.ToString() + "_Log.txt";

                if (!File.Exists(strPath))
                {
                    File.Create(strPath).Dispose();
                }

                using (StreamWriter sw = File.AppendText(strPath))
                {
                    sw.WriteLine("=============Error Logging ===========");
                    sw.WriteLine("===========Start============= " + DateTime.Now);
                    sw.WriteLine("Error Message: " + ex.Message);
                    sw.WriteLine("Error Message: " + ex.InnerException);
                    sw.WriteLine("Stack Trace: " + ex.StackTrace);
                    sw.WriteLine("===========End============= " + DateTime.Now);
                }
            }
        }

        public void BeginKnetRequest(string knetModel)
        {
            string strPath = HttpContext.Current.Server.MapPath(@"\SystemLogs\KnetPayment");

            if (!string.IsNullOrEmpty(strPath))
            {

                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strPath += "/BeginWebServiceKnetRequest_" + DateTime.Now.Ticks.ToString() + "_Log.txt";

                if (!File.Exists(strPath))
                {
                    File.Create(strPath).Dispose();
                }

                using (StreamWriter sw = File.AppendText(strPath))
                {
                    sw.WriteLine("=============Begin Knet Request ===========");
                    sw.WriteLine("===========Start============= " + DateTime.Now);
                    sw.WriteLine("KnetModel: " + knetModel);
                    sw.WriteLine("===========End============= " + DateTime.Now);
                }
            }
        }
        public static void ReadError()
        {
            string strPath = @"";
            using (StreamReader sr = new StreamReader(strPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
