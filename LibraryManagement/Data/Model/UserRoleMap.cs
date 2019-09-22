using Seagull.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class UserRoleMap
    {
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
