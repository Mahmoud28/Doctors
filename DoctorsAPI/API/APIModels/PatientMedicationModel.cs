using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Balanta.API.APIModels
{
    public class PatientMedicationModel
    {
        public int Id { get; set; }
        public int Medication { get; set; }
        public string DoctorName { get; set; }
        public int Morning { get; set; }
        public int Afternoon { get; set; }
        public string PharmacyName { get; set; }
        public string Sideeffect { get; set; }
        public List<int> SelectedIds { get; set; }
    }
}
