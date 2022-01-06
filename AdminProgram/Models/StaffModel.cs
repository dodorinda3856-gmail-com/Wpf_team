using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.Models
{
    public class StaffModel: Notifier
    {
        private int staff_id = 0;

        private string? staff_name = null;

        private string? gender = null;

        private string? medi_subject = null;

        private string? phone_num = null;

        private string? staff_email = null;

        private string? position = null;



        public int Staff_id
        {
            get { return staff_id; }
            set { this.staff_id = value; }
        }

        public string? Staff_name
        {
            get { return staff_name; }
            set { this.staff_name = value; }
        }

        public string? Gender
        {
            get { return gender; }
            set { this.gender = value; }
        }

        public string? Medi_subject
        {
            get { return medi_subject; }
            set { this.medi_subject = value; }
        }

        public string? Phone_num
        {
            get { return phone_num; }
            set { this.phone_num = value; }
        }

        public string? Staff_email
        {
            get { return staff_email; }
            set { this.staff_email = value; }
        }

        public string? Position
        {
            get { return position; }
            set { this.position = value; }
        }
    }
}
