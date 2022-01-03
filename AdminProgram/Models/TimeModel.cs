using Microsoft.Toolkit.Mvvm.ComponentModel;

/**
 * Time Table Model
 */
namespace AdminProgram.Models
{
    /**
     * 요일별로 진료보는 시간이 다름
     */
    public partial class TimeModel : ObservableObject
    {
        private int timeId;
        public int TimeId
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

        private string day; //요일
        public string Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }
    }
}
