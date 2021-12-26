using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AdminProgram.Models
{
    public partial class TMTModel : ObservableObject
    {
        public int Bueaty_Type { get; set; }
        public int Hives_Type { get; set; }
        public int Allergy_Type { get; set; }
    }
}
