using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Areas.Admin.ViewModel
{
    public class SystemSettingViewModel
    {
        //public HttpPostedFileBase File { get; set; }
        #region General 
        public string SystemLogo { get; set; }
        public string SystemContactNumber { get; set; }
        public string SystemLocation { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? FildLogInNumber { get; set; }
        #endregion

        #region Email
        public string Email { get; set; }
        public string SMTP { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }
        public string Port { get; set; }
        #endregion

        #region About Us Home Page 

        public string AboutHomeTitle { get; set; }
        public string AboutHomeTitleAR { get; set; }
        public string AboutHomeSubTitle { get; set; }
        public string AboutHomeSubTitleAR { get; set; }
        public string AboutHomeDescription { get; set; }
        public string AboutHomeDescriptionAR { get; set; }
        public string AboutHomeImage { get; set; }
        public string AboutHomeFooterDescription { set; get; }
        public string AboutHomeFooterDescriptionAr { set; get; }
        #endregion

        #region Footer
        public string FooterContactNumber { get; set; }
        public string FooterContactEmail { get; set; }
        public string FooterAdressEnglish { get; set; }
        public string FooterAdressArabic { get; set; }
        public string FooterFacebook { get; set; }
        public string FooterTwitter { get; set; }
        public string FooterLinedIn { get; set; }
        public string FooterCopyRightsEnglish { get; set; }
        public string FooterCopyRightsArabic { get; set; }
        #endregion

        #region Terms & Conditions
        public string ConditionsTitleAr { get; set; }
        public string ConditionsTitleEn { get; set; }
        public string ConditionsDescriptionAr { get; set; }
        public string ConditionsDescriptionEn { get; set; }
        #endregion

        #region Privacy & Policy
        public string PrivacyPolicyTitleAr { get; set; }
        public string PrivacyPolicyTitleEn { get; set; }
        public string PrivacyPolicyDescriptionAr { get; set; }
        public string PrivacyPolicyDescriptionEn { get; set; }
        #endregion

        #region Booking Settings
        public int BookingCharge { get; set; }
        public int MaxSeat { get; set; }
        public int EmptySeat { get; set; }
        #endregion

        #region Promotions Page
        public string PromotionTitle { get; set; }
        public string PromotionTitleAR { get; set; }
        public string PromotionDescription { get; set; }
        public string PromotionDescriptionAR { get; set; }
        public string PromotionImage { get; set; }

        #endregion


        #region Dynamic System

        public string CookieName { get; set; }

        #endregion


        public string Token { get; set; }


    }
}
