using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class AllergyViewModel
    {
      
        public int Id { get; set; }
        
        public string Name { get; set; }
        //Non Entity Prop
        public bool Selected { get; set; }

    }
}
