using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class Medication
    {
        
        public int Id { get; set; }
        
        public string Name { get; set; }

        public int UserId { get; set; }

        public bool Morning { get; set; }

        public bool Afternoon { get; set; }

        public int MorningCount { get; set; }

        public int AfternoonCount { get; set; }

        public string Days { get; set; }

        public string Doctors { get; set; }

        public string Sideeffect { get; set; }


    }
}
