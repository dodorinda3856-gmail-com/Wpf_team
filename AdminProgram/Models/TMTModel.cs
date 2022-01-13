using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AdminProgram.Models
{
    public partial class TMTModel : ObservableObject
    {
        public string TREAT_TYPE { get; set; }
        public int TREAT_COUNT { get; set; }

        public int WAIT_COUNT { get; set; }

        public string PATIENTNAME { get; set; }

        public string RESERVATIONDT { get; set; }
    }
}