using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.ViewModels
{
    public class PatientViewModel : Notifier
    {
        int patientNumber = 0;
        string userName = string.Empty;

        public int PatientNumber { get { return patientNumber; } set { patientNumber = value; } }
        public string UserName { get { return userName; } set { userName = value; } }
    }
}
