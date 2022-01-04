using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.Models
{
    public class ProcedureModel: ObservableObject
    {
        private int procedure_id;
        private int amount;
        private string procedure_name;

        public int Procedure_Id
        {
            get { return procedure_id; }
            set => SetProperty(ref procedure_id, value);
        }
        public int Amount
        {
            get { return amount; }
            set => SetProperty(ref amount, value);
        }
        public string Procedure_Name
        {
            get { return procedure_name; }
            set => SetProperty(ref procedure_name, value);
        }
    }
}
