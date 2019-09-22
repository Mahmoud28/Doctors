using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Helper.URLShortner
{
    public class Password
    {
        public int Id { set; get; }
        public int UserId { set; get; }
        public string OldPassword { set; get; }
        public int URLId { set; get; }
        public DateTime UpdatedDate { set; get; }


    }
}
