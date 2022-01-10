using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AdminProgram.Models
{
    public class MediStaffModel : ObservableObject
    {
        //스태프(진료진) id
        private int staffId;
        public int StaffId
        {
            get => staffId;
            set => SetProperty(ref staffId, value);
        }

        //스태프(진료진) 이름
        private string staffName;
        public string StaffName
        {
            get => staffName;
            set => SetProperty(ref staffName, value);
        }

        //스태프(진료진) 성별
        private string gender;
        public string Gender 
        { 
            get => gender;
            set => SetProperty(ref gender, value);
        }

        //스태프(진료진) 담당 과목
        private string mediSubject;
        public string MediSubject
        {
            get => mediSubject;
            set => SetProperty(ref mediSubject, value);
        }

        //스태프(진료진) 전화번호
        private string staffPhoneNum;
        public string StaffPhoneNum
        {
            get => staffPhoneNum;
            set => SetProperty(ref staffPhoneNum, value);
        }

        //스태프(진료진) 이메일
        private string staffEmail;
        public string StaffEmail
        {
            get => staffEmail;
            set => SetProperty(ref staffEmail, value);
        }

        //스태프(진료진) 직급
        private string position;
        public string Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }
        
    }
}
