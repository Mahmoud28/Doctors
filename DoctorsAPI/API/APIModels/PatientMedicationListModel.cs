using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Balanta.API.APIModels
{
    public class PatientMedicationListModel
    {
        public int Id { get; set; }
        public string Medicines { get; set; }
        public string Doctor { get; set; }
        public string Days { get; set; }
    }
}
