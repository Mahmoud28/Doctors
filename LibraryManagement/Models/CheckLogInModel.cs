using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Models
{
    public class CheckUserAccountModel
    {
        public CheckUserAccountModel()
        {
            //ErrorMessage = new List<string>();
        }
        public UserViewModel user { get; set; }
        public string ErrorMessage { get; set; }
    }
}
