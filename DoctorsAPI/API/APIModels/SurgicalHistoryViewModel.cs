using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Balanta.API.APIModels
{
    public class SurgicalHistoryViewModel
    {
        public SurgicalHistoryViewModel()
        {
            //Images = new Microsoft.AspNetCore.Http.Internal.FormFile;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public long SurgicalDate { get; set; }
        public int UserId { get; set; }
        public IFormFile[] Images { get; set; }
        //public IFormFile Images { get; set; }
    }
}
