using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class Pharmacy
    {
       
        public int Id { get; set; }
        
        public string Name { get; set; }

        public int MedicationId { get; set; }

        
    }
}
