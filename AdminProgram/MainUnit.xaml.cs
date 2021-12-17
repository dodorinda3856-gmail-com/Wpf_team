using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace AdminProgram
{
    /// <summary>
    /// MainUnit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainUnit : Window
    {
        public MainUnit()
        {
            InitializeComponent();
        }

        bool isMenuAction = false;   // 메뉴 이동중인지 확인
        bool isMenuShow = false;   // 메뉴 이동중인지 확인
        void ActMenu(string strTargetName, bool isMouseOver)
        {
            if (!isMenuAction)   // 동작중인지 움직이지 않는다.
            {
                isMenuAction = true;

                ThicknessAnimation ThickAni = new();
                ThickAni.BeginTime = new TimeSpan(0);   // 시작시간
                ThickAni.SetValue(Storyboard.TargetNameProperty, strTargetName);
                Storyboard.SetTargetProperty(ThickAni, new PropertyPath(MarginProperty));

                Thickness ThickOpen = new(0, 0, 0, 0);
                Thickness ThickClose = new(-200, 0, 0, 0);

                if (isMouseOver)
                {
                    isMenuShow = true;
                    ThickAni.From = ThickClose;
                    ThickAni.To = ThickOpen;
                }
                else
                {
                    isMenuShow = false;
                    ThickAni.From = ThickOpen;
                    ThickAni.To = ThickClose;
                }
                ThickAni.By = new Thickness(-200, 0, 0, 0);
                ThickAni.DecelerationRatio = 0.8;
                ThickAni.SpeedRatio = 10;
                ThickAni.AccelerationRatio = 0.2;
                ThickAni.Duration = new Duration(new TimeSpan(0, 0, 0, 2, 0));   // 일,시,분,초,밀리초)

                Storyboard sb = new();
                sb.Children.Add(ThickAni);
                sb.Completed += Storyboard_Completed;
                sb.Begin(this);
            }
        }

        // 스토리보드 종료
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            // 액션 완료
            isMenuAction = false;

            // 액션 완료 후 마우스가 위에 없다면 메뉴를 숨긴다.
            if (isMenuShow && !stpMenu.IsMouseOver)
            {
                ActMenu("stpMenu", false);
            }
        }

        private void StpMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            ActMenu("stpMenu", stpMenu.IsMouseOver);
        }
        private void StpMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            ActMenu("stpMenu", stpMenu.IsMouseOver);
        }

        private void MediAppointment_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/MediAppointmentPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Home_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("StartPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Medi_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/TreatmentPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Patient_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/PatientPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Staff_Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.frame.Navigate(new Uri("Views/StaffPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
