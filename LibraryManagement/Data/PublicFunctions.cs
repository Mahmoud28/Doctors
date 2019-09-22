using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Seagull.Doctors.Data
{

    public static class PublicFunctions
    {
        public static IConfiguration Configuration { get; set; }
        private static string keySecret { get; set; } = @"3048 0241 00C9 18FA CF8D EB2D EFD5 FD37 89B9 E069 EA97 FC20 5E35 F5
                                            77 EE31 C4FB C6E4 4811 7D86 BC8F BAFA 362F 922B F01B 2F40 C744 2654 
                                            C0DD 2881 D673 CA2B 4003 C266 E2CD CB02 0301 0001 Z0R1 QT0L";
        public static long ConvertToTimestamp(DateTime value)
        {
            var date = value;
            long timestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            timestamp /= TimeSpan.TicksPerSecond;
            return timestamp;
        }
        public static DateTime ConvertTimestampToDateTime(long Timestamp)
        {
            DateTime origin = Convert.ToDateTime(new DateTime(1970, 1, 1, 0, 0, 0).ToString("yyyy/MM/dd"));
            return TimeZoneInfo.ConvertTimeToUtc(origin.AddSeconds(Timestamp));
        }
        public static bool IsBewteenTwoDates(DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }
        public static string GetRandomAlphanumericString(int length)
        {
            const string alphanumericCharacters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";
            return GetRandomString(length, alphanumericCharacters);
        }
        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8)
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");
            var bytes = new byte[length * 8];
            var result = new char[length];
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(bytes);
            }
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
        public static void SendNotification(string message, string token,bool ToAllUser)
        {
            try
            {
                var applicationID = "AAAATyLZSZ0:APA91bGGTRwnropKbVlIIEgGSZlLBdWjZtkMKjRVe3Ru6im0oABcNwrQx3yKp3qq2kQ8n4XBtgHZWXFvEHiPlhzdY7mwQl6praQTBH3tmLLjxC_MSw8R4--xLdPCKQ1WlPDDa8XA2w9m";
                var senderId = "339887081885";
                string deviceId;
                if (ToAllUser)
                     deviceId = "/topics/all";
                else
                    deviceId = token;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = message,
                        title = "Balan6a",
                        priority = 10//  icon = "myicon"
                    }
                };
                //var serializer = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                //    tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }
        public static string ConvertTo64Bit(IFormFile file)
        {
            string s;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                s = Convert.ToBase64String(fileBytes);
            }
            return "data:image/jpeg;base64," + s;
        }
        public static string RandomPassword(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        public static string Encrypt(string text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keySecret));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }
        public static string Decrypt(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keySecret));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }


        // Create QR Code Pass Text To Encrypted and drowning to the Image As BitMap and save it as .jpg Image
        public static string GenerateQRCode(string qrcodeText,int? userId)
        {
            string folderPath = "";
           if (userId == null)
                folderPath = Directory.GetCurrentDirectory() + @"\wwwroot\Upload\" + @"GuestQrCodesImage\" + qrcodeText + @"\";
            else
                folderPath = Directory.GetCurrentDirectory() + @"\wwwroot\Upload\" + @"ImagesQrCodes\" + qrcodeText + @"\" + userId + @"\";
            string imagePath = folderPath+ @"\QrCode.Jpeg";
            

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var barcodeWriter = new BarcodeWriter()
            {
                Format = BarcodeFormat.QR_CODE,
                //Format = BarcodeFormat.AZTEC,
                //Format = BarcodeFormat.CODE_128,
                //Format = BarcodeFormat.PDF_417,
                Options = new QrCodeEncodingOptions()
                {
                    CharacterSet = "UTF-8",
                    ErrorCorrection = ErrorCorrectionLevel.H,
                    Height = 250,
                    Width = 250,
                },
            };

            var result = barcodeWriter.Write(Encrypt(qrcodeText));

            string barcodePath = imagePath;
            var barcodeBitmap = new Bitmap(result);
            
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }
        public static string ReadQRCode()
        {
            string qrcodeText = "";
            string imagePath = Directory.GetCurrentDirectory() + @"\wwwroot\Upload\" + @"ImagesQrCodes\QrCode.Jpeg";
            string barcodePath = (imagePath);
            var barcodeReader = new BarcodeReader();

            var result = barcodeReader.Decode(new Bitmap(barcodePath));
            if (result != null)
            {
                qrcodeText = result.Text;
            }
            //var model = Decrypt(barcodeText).Replace("\r", string.Empty);
            //model = model.Replace("\t", string.Empty);
            //model = model.Replace("\n", string.Empty);
            //model = model.Replace(" ", string.Empty);

            return Decrypt(qrcodeText);
        }
    }
}
