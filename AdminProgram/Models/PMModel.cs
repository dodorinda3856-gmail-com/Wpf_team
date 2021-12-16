using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.ViewModels
{
    public class PMModel: Notifier
    {
        private int patient_ID = 0;
        private string? resident_Regist_Num = null;
        private string? address = null;
        private string? patient_Name = null;
        private string? phone_Num = null;
        private DateTime regist_Date;
        private string? gender = null;
        private DateTime dob;
        private int age = 0;

        public int Patient_ID 
        {
            get { return patient_ID; }
            set { this.patient_ID = value; }
        }
        public string? Resident_Regist_Num
        {
            get { return resident_Regist_Num; }
            set { this.resident_Regist_Num = value; }
        }

        public string? Address
        {
            get { return address; }
            set { this.address = value; }
        }

        public string? Patient_Name
        {
            get { return patient_Name; }
            set { this.patient_Name = value; }
        }

        public string? Phone_Num
        {
            get { return phone_Num; }
            set { this.phone_Num = value; }
        }

        public DateTime Regist_Date
        {
            get { return regist_Date; }
            set { this.regist_Date = value; }
        }

        public string? Gender
        {
            get { return gender; }
            set { this.gender = value; }
        }
        public int Age
        {
            get { return age; }
            set { this.age = value; }
        }

        public DateTime Dob
        {
            get { return dob; }
            set { this.dob = value; }
        }

    }
}
