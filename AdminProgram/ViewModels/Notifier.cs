using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram
{
    public class Notifier : INotifyPropertyChanged
    {
        //이벤트는 대리자를 event 한정자로 수식해서 만듬
        //이벤트 = event 대리자
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            //속성의 데이터가 변경되는 시점에 알려주는 이벤트
            //데이터와 화면의 실시간 동기화에 중요한 역할
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
