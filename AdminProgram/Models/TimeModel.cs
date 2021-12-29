using Microsoft.Toolkit.Mvvm.ComponentModel;

/**
 * Time Table Model
 */
namespace AdminProgram.Models
{
    public partial class TimeModel : ObservableObject
    {
        private string timeId;
        public string TimeId
        {
            get => timeId;
            set => SetProperty(ref timeId, value);
        }

        private string hour;
        public string Hour
        {
            get => hour;
            set => SetProperty(ref hour, value);
        }

        private string day;
        public string Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }
    }
}
