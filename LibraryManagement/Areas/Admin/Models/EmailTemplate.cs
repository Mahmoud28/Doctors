using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class EmailTemplate
    {

        public int Id { get; set; }
        public string TemplateNameAr { get; set; }
        public string TemplateNameEn { get; set; }
        public string BodyAr { get; set; }
        public string BodyEn { get; set; }
        public string HeaderAr { get; set; }
        public string HeaderEn { get; set; }
        public int ServiceType { get; set; }

    }
}
