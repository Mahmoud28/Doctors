using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;

namespace Seagull.Doctors.Helper.ImageHelper
{
    public class ImageHelper
    {
        private readonly Paths _paths = new Paths();

        public void SaveByteArrayAsImage(string uploadFolderName, string fullOutputPath, string base64String, string type = "")
        {
            // string _fullPath = _paths._mainDirectory + uploadFolderName + fullOutputPath;
            string _fullPath = _paths._mainDirectory + uploadFolderName + fullOutputPath;
            if (!Directory.Exists(_paths._mainDirectory))
            {
                Directory.CreateDirectory(_paths._mainDirectory);
            }

            if (!Directory.Exists(_paths._mainDirectory + uploadFolderName))
            {
                Directory.CreateDirectory(_paths._mainDirectory + uploadFolderName);
            }

            switch (type)
            {
                default:
                    base64String = base64String.Replace("data:image/jpeg;base64,", "");
                    base64String = base64String.Replace("data:image/png;base64,", "");
                    // Convert Base64 String to byte[]
                    byte[] imageBytes = Convert.FromBase64String(base64String);
                    MemoryStream ms = new MemoryStream(imageBytes, 0,
                      imageBytes.Length);
                    // Convert byte[] to Image
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    Image image = Image.FromStream(ms, true);
                    image.Save(_fullPath, ImageFormat.Jpeg);
                    break;
                case "Video":
                    base64String = base64String.Replace("data:video/mp4;base64,", "");
                    using (FileStream fs = new FileStream(_fullPath, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {

                            byte[] data = Convert.FromBase64String(base64String);
                            bw.Write(data, 0, data.Length);
                            bw.Close();
                        }
                    }
                    break;
            }
        }
        public string ConvartTo64Bit(IFormFile file)
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
    }

    public class Paths
    {
        public Paths()
        {
            _mainDirectory = Directory.GetCurrentDirectory() + @"\wwwroot\Upload\";
            _mainDirectoryImages = Directory.GetCurrentDirectory() + @"\wwwroot";
            _mainBalan6aAPIDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) + @"\wwwroot\Upload\";
            _mainBalan6aAPIDirectoryImages = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) + @"\wwwroot";
            _CreateEventImages =  Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) + @"\LibraryManagement\wwwroot";
            _personalImage = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) + @"\LibraryManagement\wwwroot";
            _insuranceImage = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) + @"\LibraryManagement\wwwroot";
            SurgicalHistoryImages = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName) + @"\LibraryManagement\wwwroot\Upload\SurgicalHistory\";
        }
        public string _mainDirectory { get; set; }
        public string _mainDirectoryImages { get; set; }
        
        public string _mainBalan6aAPIDirectory { get; set; }
        public string _mainBalan6aAPIDirectoryImages { get; set; }

        public string _CreateEventImages { get; set; }
        public string _insuranceImage { get; set; }
        public string SurgicalHistoryImages { get; set; }
        public string _personalImage { get; set; }

        public void CreateBaln6aPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }       
    }

    public static class FormFileExtensions
    {
        //public const int ImageMinimumBytes = 512;

        public static bool IsImage(this IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }
            return true;
        }
    }
}
