using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class CreateEventPlayCategorieAPIViewModel
    {
        public int Id { get; set; }
        public int? CreateEventPlayId { get; set; }
        public int? CategorieId { get; set; }
    }
}
