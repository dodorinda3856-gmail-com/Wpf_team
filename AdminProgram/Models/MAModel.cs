using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AdminProgram.Models
{
    public partial class MAModel : ObservableObject //속성 변경 알림을 지원해야하는 모든 객체에 대한 시작점으로 사용할 수 있음
    {
        private string staffName;
        public string StaffName 
        { 
            get => staffName; 
            set => SetProperty(ref staffName, value); 
        }

        private string patientName;
        public string PatientName 
        { 
            get => patientName; 
            set => SetProperty(ref patientName, value); 
        }

        private string treatStatusVal;
        public string TreatStatusVal 
        { 
            get => treatStatusVal; 
            set => SetProperty(ref treatStatusVal, value); 
        }
    }
}
