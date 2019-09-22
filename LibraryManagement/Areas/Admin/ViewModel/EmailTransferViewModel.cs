using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.ViewModel
{
    public class EmailTransferViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime? SendDate { get; set; }
        public int? UserCounts { get; set; }
        public object Child { get; set; }


        ////
        //public string strVacancy { get; set; }
        //public string strVacancyStatus { get; set; }
        //public string strUser { get; set; }
        //public object Child { get; set; }
        //public List<int> Users { set;get;}

        ////Email par Users
        //public string UserEmail { get; set; }
        //public string UserFullName { get; set; }
        //public string EmailStatus { get; set; }


    }
}
