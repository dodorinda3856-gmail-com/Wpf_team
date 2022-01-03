using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.ViewModels
{
    public class MSModel: Notifier
    {
        private string? staff_login_id = null;
        private string? staff_login_pw = null;
        private int staff_id = 0;

        public string Staff_Login_id { get; set; }
        public string Staff_Loing_Pw { get; set; }
        public int Staff_Id { get; set; }
    }
}
