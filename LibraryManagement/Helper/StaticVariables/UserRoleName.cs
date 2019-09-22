using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Helper.StaticVariables
{
    public class UserRoleName
    {

        #region Routing Users
        public const string User = "Register User";
        public const string Admin = "Admin";
        public const string Guest = "Guest";
        public const string Producers = "Producers";
        public const string TicketingUser = "Ticketing User";
        public const string OrganizersUser = "Organizers User";
        #endregion
    }

    public class RegexStrings {
        public const string MobileRegex = @"^[0-9]{8}$";
        //public const string MobileRegex = @"^([0]|[00]|[+] *\)?)[0-9]{8}$";
        public const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public const string UrlRegex = @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)";
    }
}
