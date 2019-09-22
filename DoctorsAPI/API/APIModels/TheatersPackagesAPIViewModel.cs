using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class TheatersPackagesAPIViewModel/* : APIBaseModel*/
    {
        public int Id { get; set; }
       public int TheaterId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
      
    }
}
