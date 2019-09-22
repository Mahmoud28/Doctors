
using Seagull.Doctors.Areas.Admin.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.ViewModel
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            
        }
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "SelectedUserRoles")]
        public string SelectedUserRoles { get; set; }

        [Required]
        [Display(Name = "SelectedUserRoles")]
        public string SelectedProducersUser { get; set; }


        [Display(Name = "LangId")]
        public int? LangId { get; set; }

        public int? ProducerId { get; set; }
        public int? FildLogIn { get; set; }

        public bool? Activation { get; set; }

        public bool? IsDeleted { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? DeletedBy { get; set; }
        public object Child { get; set; }

        [Display(Name = "IsRtl")]
        public bool IsRtl { get; set; }

       

        public virtual List<string> PermissionName { get; set; }

        public virtual List<string> UserRoleName { get; set; }

        public virtual List<string> UserProducersName { get; set; }

        public bool IsProducersUser { get; set; }

        public int? CachKey { get; set; }

        

    }

    public class UserRegisterModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Required")]
       

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Email Address DataType")]
        public string Email { get; set; }
      
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Mobile")]
        [DataType(DataType.PhoneNumber)]
        public string Mobile { get; set; }


        [Required(ErrorMessage = "Required")]
        public bool AcceptCondition { get; set; }

        public int? LangId { get; set; }

        public string returnUrl { set; get; }

        [Display(Name = "Gender")]
        public bool? Gender { get; set; }

        public int Age { get; set; }

    }

    public class UserViewJoinUsViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
       // public int Category { get; set; }
    }
}
