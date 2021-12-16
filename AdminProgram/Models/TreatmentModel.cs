using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.Models
{
    public class TreatmentModel //: Notifier
    {
        public int PatientNumber { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public string TreatDetail { get; set; }
    }
}
