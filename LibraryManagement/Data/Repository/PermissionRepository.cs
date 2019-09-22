using CodeBureau;
using ExtensionMethods;
using Newtonsoft.Json.Linq;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Helpers.WhereOperation;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Model;
using Seagull.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seagull.Core.Models;
using Seagull.Core.Helper;

namespace Seagull.Core.Data.Repository
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        private readonly IGlobalSettings _globalSettings;
        public PermissionRepository(LibraryDbContext context, IGlobalSettings globalSettings) : base(context)
        {
            _globalSettings = globalSettings;

        }

        public Permission GetById(int id)
        {
            return _context.Set<Permission>().Where(d => d.Id == id).FirstOrDefault();

        }

        public List<PermissionRecordModelAngular> GetAllPermissionRecordForTree()
        {
            List<PermissionRecordModelAngular> tree = new List<PermissionRecordModelAngular>();
            tree = (from a in _context.Set<Permission>().ToList()
                    group a by a.Category into g
                    select new PermissionRecordModelAngular
                    {
                        title = g.FirstOrDefault().Category,
                        childs = (from c in g
                                  select new PermissionRecordModelAngularChild
                                  {
                                      id = c.Id,
                                      title = c.Name
                                  }).ToList()
                    }).ToList();
            return tree;
        }

        public bool CheckAccess(string PermName)
        {
            bool isAllow = _context.Set<User>().Include(ur => ur.fk_UserRoleMap).Where(a => a.Id == _globalSettings.CurrentUser.Id).Where(r => r.fk_UserRoleMap.Where(e => e.UserRole.fk_UserRolePermMap.Where(p => p.Permission.Name.Equals(PermName)).Count() > 0).Count() > 0).Count() > 0;

            return isAllow;
        }

        public bool CheckCacheKey()
        {
            bool isAllow = true;
            if (_globalSettings.CurrentUser == null )//|| _globalSettings.CurrentUser.CachKey == 0)
                isAllow = false;
            return isAllow;
        }
    }
}
