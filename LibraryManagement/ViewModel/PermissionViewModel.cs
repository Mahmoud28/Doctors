using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.ViewModel
{
    public class PermissionViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }
    }
    public partial class PermissionRecordModelAngular
    {
        public PermissionRecordModelAngular()
        {
            childs = new List<PermissionRecordModelAngularChild>();
        }
        public string title { get; set; }
        public List<PermissionRecordModelAngularChild> childs { get; set; }
    }
    public class PermissionRecordModelAngularChild
    {
        public int id { get; set; }
        public string title { get; set; }
    }
}
