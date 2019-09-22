using Microsoft.EntityFrameworkCore;
using Seagull.Core.Areas.Admin.Models;
using Seagull.Core.Areas.Admin.ViewModel;
using Seagull.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace Seagull.Core.Data.MappingExtension
{
    public class SystemSettingMapping
    {
        private IConfigurationBuilder _builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        public static IConfiguration _Configuration { get; set; }
        public SystemSettingMapping()
        {
            _Configuration = _builder.Build();

        }
        public SystemSettingViewModel ToModel(DbSet<SystemSetting> systemSetting , string type)
        {
            SystemSettingViewModel model = new SystemSettingViewModel();
            model.SystemLogo = systemSetting.Where(a => a.Name == "SystemLogo").FirstOrDefault().Value;
            model.FildLogInNumber = Convert.ToInt32(systemSetting.Where(a => a.Name == "FildLogInNumber").FirstOrDefault().Value);
            model.SystemContactNumber = systemSetting.Where(a => a.Name == "SystemContactNumber").FirstOrDefault().Value;
            model.SystemLocation = systemSetting.Where(a => a.Name == "SystemLocation").FirstOrDefault().Value;
            model.Latitude   = systemSetting.Where(a => a.Name == "Latitude").FirstOrDefault().Value;
            model.Longitude = systemSetting.Where(a => a.Name == "Longitude").FirstOrDefault().Value;
            model.Email = systemSetting.Where(a => a.Name == "Email").FirstOrDefault().Value;
            model.SMTP = systemSetting.Where(a => a.Name == "SMTP").FirstOrDefault().Value;
            model.EmailUserName = systemSetting.Where(a => a.Name == "EmailUserName").FirstOrDefault().Value;
            model.EmailPassword = systemSetting.Where(a => a.Name == "EmailPassword").FirstOrDefault().Value;
            model.Port = systemSetting.Where(a => a.Name == "Port").FirstOrDefault().Value;
            model.FooterContactNumber = systemSetting.Where(a => a.Name == "FooterContactNumber").FirstOrDefault().Value;
            model.FooterContactEmail = systemSetting.Where(a => a.Name == "FooterContactEmail").FirstOrDefault().Value;
            model.FooterAdressEnglish = systemSetting.Where(a => a.Name == "FooterAdressEnglish").FirstOrDefault().Value;
            model.FooterAdressArabic = systemSetting.Where(a => a.Name == "FooterAdressArabic").FirstOrDefault().Value;
            model.FooterFacebook = systemSetting.Where(a => a.Name == "FooterFacebook").FirstOrDefault().Value;
            model.FooterTwitter = systemSetting.Where(a => a.Name == "FooterTwitter").FirstOrDefault().Value;
            model.FooterLinedIn = systemSetting.Where(a => a.Name == "FooterLinedIn").FirstOrDefault().Value;
            
            model.FooterCopyRightsEnglish = systemSetting.Where(a => a.Name == "FooterCopyRightsEnglish").FirstOrDefault().Value;
            model.FooterCopyRightsArabic = systemSetting.Where(a => a.Name == "FooterCopyRightsArabic").FirstOrDefault().Value;
            model.AboutHomeTitle = systemSetting.Where(a => a.Name == "AboutHomeTitle").FirstOrDefault().Value;
            model.AboutHomeTitleAR = systemSetting.Where(a => a.Name == "AboutHomeTitleAR").FirstOrDefault().Value;
            model.AboutHomeSubTitle = systemSetting.Where(a => a.Name == "AboutHomeSubTitle").FirstOrDefault().Value;
            model.AboutHomeSubTitleAR = systemSetting.Where(a => a.Name == "AboutHomeSubTitleAR").FirstOrDefault().Value;
            model.AboutHomeDescription = systemSetting.Where(a => a.Name == "AboutHomeDescription").FirstOrDefault().Value;
            model.AboutHomeDescriptionAR = systemSetting.Where(a => a.Name == "AboutHomeDescriptionAR").FirstOrDefault().Value;
            model.AboutHomeImage = systemSetting.Where(a => a.Name == "AboutHomeImage").FirstOrDefault().Value;
            model.AboutHomeFooterDescription = systemSetting.Where(a => a.Name == "AboutHomeFooterDescription").FirstOrDefault().Value;
            model.AboutHomeFooterDescriptionAr = systemSetting.Where(a => a.Name == "AboutHomeFooterDescriptionAr").FirstOrDefault().Value;


            model.ConditionsDescriptionAr = systemSetting.Where(a => a.Name == "ConditionsDescriptionAr").FirstOrDefault().Value;
            model.ConditionsDescriptionEn = systemSetting.Where(a => a.Name == "ConditionsDescriptionEn").FirstOrDefault().Value;

            //model.PrivacyPolicyTitleAr = systemSetting.Where(a => a.Name == "PrivacyPolicyTitleAr").FirstOrDefault().Value;
            //model.PrivacyPolicyTitleEn = systemSetting.Where(a => a.Name == "PrivacyPolicyTitleEn").FirstOrDefault().Value;
            model.PrivacyPolicyDescriptionAr = systemSetting.Where(a => a.Name == "PrivacyPolicyDescriptionAr").FirstOrDefault().Value;
            model.PrivacyPolicyDescriptionEn = systemSetting.Where(a => a.Name == "PrivacyPolicyDescriptionEn").FirstOrDefault().Value;

            model.BookingCharge =Convert.ToInt32(systemSetting.Where(a => a.Name == "BookingCharge").FirstOrDefault().Value);
            model.MaxSeat = Convert.ToInt32(systemSetting.Where(a => a.Name == "MaxSeat").FirstOrDefault().Value);
            model.EmptySeat = Convert.ToInt32(systemSetting.Where(a => a.Name == "EmptySeat").FirstOrDefault().Value);
            model.PromotionTitle = systemSetting.Where(a => a.Name == "PromotionTitle").FirstOrDefault().Value;
            model.PromotionTitleAR = systemSetting.Where(a => a.Name == "PromotionTitleAR").FirstOrDefault().Value;
            model.PromotionDescription = systemSetting.Where(a => a.Name == "PromotionDescription").FirstOrDefault().Value;
            model.PromotionDescriptionAR = systemSetting.Where(a => a.Name == "PromotionDescriptionAR").FirstOrDefault().Value;
            model.PromotionImage = systemSetting.Where(a => a.Name == "PromotionImage").FirstOrDefault().Value;

            model.CookieName = _Configuration["System:CookieName"];
            //model.WebSiteVisitor = systemSetting.Where(a => a.Name == "WebSiteVisitor").FirstOrDefault().Value;

            return model;
        }

        public void ToEntity(LibraryDbContext context , SystemSettingViewModel model, string type)
        {
            DbSet<SystemSetting> systemSetting = context.SystemSettings;
            dynamic entity = null;

            #region Global
            entity = systemSetting.Where(a => a.Name == "SystemLogo").FirstOrDefault();
            entity.Value = model.SystemLogo;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "SystemContactNumber").FirstOrDefault();
            entity.Value = model.SystemContactNumber;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "SystemLocation").FirstOrDefault();
            entity.Value = model.SystemLocation;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "Latitude").FirstOrDefault();
            entity.Value = model.Latitude;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "Longitude").FirstOrDefault();
            entity.Value = model.Longitude;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FildLogInNumber").FirstOrDefault();
            entity.Value = model.FildLogInNumber.ToString();
            systemSetting.Update(entity);


            #endregion

            #region Email 
            entity = systemSetting.Where(a => a.Name == "Email").FirstOrDefault();
            entity.Value = model.Email;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "SMTP").FirstOrDefault();
            entity.Value = model.SMTP;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "EmailUserName").FirstOrDefault();
            entity.Value = model.EmailUserName;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "EmailPassword").FirstOrDefault();
            entity.Value = model.EmailPassword;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "Port").FirstOrDefault();
            entity.Value = model.Port;
            systemSetting.Update(entity);
            #endregion 

            #region Footer 
            

            entity = systemSetting.Where(a => a.Name == "FooterContactNumber").FirstOrDefault();
            entity.Value = model.FooterContactNumber;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FooterContactEmail").FirstOrDefault();
            entity.Value = model.FooterContactEmail;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FooterAdressEnglish").FirstOrDefault();
            entity.Value = model.FooterAdressEnglish;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FooterAdressArabic").FirstOrDefault();
            entity.Value = model.FooterAdressArabic;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FooterFacebook").FirstOrDefault();
            entity.Value = model.FooterFacebook;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FooterTwitter").FirstOrDefault();
            entity.Value = model.FooterTwitter;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FooterLinedIn").FirstOrDefault();
            entity.Value = model.FooterLinedIn;
            systemSetting.Update(entity);
            
            entity = systemSetting.Where(a => a.Name == "FooterCopyRightsArabic").FirstOrDefault();
            entity.Value = model.FooterCopyRightsArabic;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "FooterCopyRightsEnglish").FirstOrDefault();
            entity.Value = model.FooterCopyRightsEnglish;
            systemSetting.Update(entity);
            #endregion

            #region About Us Home Page 

            entity = systemSetting.Where(a => a.Name == "AboutHomeTitle").FirstOrDefault();
            entity.Value = model.AboutHomeTitle;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeTitleAR").FirstOrDefault();
            entity.Value = model.AboutHomeTitleAR;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeSubTitle").FirstOrDefault();
            entity.Value = model.AboutHomeSubTitle;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeSubTitleAR").FirstOrDefault();
            entity.Value = model.AboutHomeSubTitleAR;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeDescription").FirstOrDefault();
            entity.Value = model.AboutHomeDescription;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeDescriptionAR").FirstOrDefault();
            entity.Value = model.AboutHomeDescriptionAR;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeImage").FirstOrDefault();
            entity.Value = model.AboutHomeImage;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeFooterDescription").FirstOrDefault();
            entity.Value = model.AboutHomeFooterDescription;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "AboutHomeFooterDescriptionAr").FirstOrDefault();
            entity.Value = model.AboutHomeFooterDescriptionAr;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "ConditionsDescriptionAr").FirstOrDefault();
            entity.Value = model.ConditionsDescriptionAr;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "ConditionsDescriptionEn").FirstOrDefault();
            entity.Value = model.ConditionsDescriptionEn;
            systemSetting.Update(entity);

           
            
            entity = systemSetting.Where(a => a.Name == "PrivacyPolicyDescriptionAr").FirstOrDefault();
            entity.Value = model.PrivacyPolicyDescriptionAr;
            systemSetting.Update(entity);
            entity = systemSetting.Where(a => a.Name == "PrivacyPolicyDescriptionEn").FirstOrDefault();
            entity.Value = model.PrivacyPolicyDescriptionEn;
            systemSetting.Update(entity);

            #endregion

            #region Promotion Page 

            entity = systemSetting.Where(a => a.Name == "PromotionTitle").FirstOrDefault();
            entity.Value = model.PromotionTitle;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "PromotionTitleAR").FirstOrDefault();
            entity.Value = model.PromotionTitleAR;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "PromotionDescription").FirstOrDefault();
            entity.Value = model.PromotionDescription;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "PromotionDescriptionAR").FirstOrDefault();
            entity.Value = model.PromotionDescriptionAR;
            systemSetting.Update(entity);

            entity = systemSetting.Where(a => a.Name == "PromotionImage").FirstOrDefault();
            entity.Value = model.PromotionImage;
            systemSetting.Update(entity);

          
            #endregion
            context.SaveChanges();
        }

       
        public string Get64Image(string _path , string type)
        {
            if (!File.Exists(_path))
                return string.Empty;

            string base64String = string.Empty;

            switch (type)
            {
                default:
                    using (Image image = Image.FromFile(_path))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            // Convert byte[] to Base64 String
                            base64String = Convert.ToBase64String(imageBytes);
                        }
                    }
                    break;
                case "LastSectionImage.mp4":
                case "Video":
                    Byte[] bytes = File.ReadAllBytes(_path);
                    base64String = Convert.ToBase64String(bytes);
                    break;
            }
            return base64String;

        }
    }
}
