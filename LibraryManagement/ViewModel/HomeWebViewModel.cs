using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Areas.Admin.ViewModel;
using Seagull.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.ViewModel
{
    public class HomeWebViewModel
    {
        public HomeWebViewModel()
        {
            slider = new List<SliderViewModel>();
            JustInShowsModel = new List<ShowsModel>();
            showingNow = new List<ShowsModel>();
            AllShows = new List<ShowsModel>();
        }

        public List<SliderViewModel> slider { get; set; }
        public List<ShowsModel> JustInShowsModel { set; get; }
        public List<ShowsModel> showingNow { set; get; }
        public List<ShowsModel> AllShows { set; get; }
        #region About Home Page
        public string AboutTitle { get; set; }
        public string AboutTitleAR { get; set; }
        public string AboutSubTitle { get; set; }
        public string AboutSubTitleAR { get; set; }
        public string AboutDescription { get; set; }
        public string AboutDescriptionAR { get; set; }
        public string AboutHomeFooterDescription { get; set; }
        public string AboutHomeFooterDescriptionAr { get; set; }
        public string AboutImage { get; set; }

        #endregion

        #region Contact Home Page 

        public string ContactTitle { get; set; }
        public string ContactTitleAR { get; set; }
        public string ContactSubTitle { get; set; }
        public string ContactSubTitleAR { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        #endregion


        #region TermsAndConditions 

        public string TermsandConditionArabic { get; set; }
        public string TermsandConditionEnglish { get; set; }
        #endregion

        #region Ploicy 

        public string PrivacyPolicyEnglish { get; set; }
        public string PrivacyPolicyArabic { get; set; }
        #endregion

        #region Promotions Page
        public string PromotionTitle { get; set; }
        public string PromotionTitleAR { get; set; }
        public string PromotionDescription { get; set; }
        public string PromotionDescriptionAR { get; set; }
        public string PromotionImage { get; set; }

        #endregion

    }

    public class ShowsModel
    {
        public int Id { set; get; }
        public string Image { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public string VideoID { set; get; }
        public string Duration { set; get; }
        public string Date { set; get; }
        public string Time { set; get; }

        public bool Status { set; get; } = true;
    }

    public class Banner
    {
        public Banner()
        {
            Banners = new List<string>();
            ShowsModels = new List<ShowsModel>();
        }
        public List<ShowsModel> ShowsModels { set; get; }
        public List<String> Banners { set; get; }
    }

}
