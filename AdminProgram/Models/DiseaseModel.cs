using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.Models
{
    public class DiseaseModel: ObservableObject
    {
        private int disease_id;
        private string disease_name;

        public int DiseaseId
        {
            get { return disease_id; }
            set => SetProperty(ref disease_id, value);
        }
        public string DiseaseName
        {
            get { return disease_name; }
            set => SetProperty(ref disease_name, value);
        }
    }
}
