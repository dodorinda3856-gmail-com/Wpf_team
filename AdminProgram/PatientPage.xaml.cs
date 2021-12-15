using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminProgram
{
    /// <summary>
    /// PatientPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PatientPage : Page
    {
        
        public PatientPage()
        {
            InitializeComponent();

            List<User> users = new List<User>();
            users.Add(new User() {Chartnum = 00001, patientnum = 1111, Name = "이성찬", Sex = true, Age = 27, SecurityNum = "9509281000000", Birthday = 950928, RecentVisit = "21/12/15", Address = "서울시 양천구 목동"});
            users.Add(new User() {Chartnum = 00001, patientnum = 1111, Name = "이성찬", Sex = true, Age = 27, SecurityNum = "9509281000000", Birthday = 950928, RecentVisit = "21/12/15", Address = "서울시 양천구 목동"});
            users.Add(new User() {Chartnum = 00001, patientnum = 1111, Name = "이성찬", Sex = true, Age = 27, SecurityNum = "9509281000000", Birthday = 950928, RecentVisit = "21/12/15", Address = "서울시 양천구 목동"});
            dataGrid.ItemsSource = users;
        }

        private void selectedDate(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                MessageBox.Show("No Date");
            }
            else
            {
                //날짜 가져오는 부분
                MessageBox.Show(date.Value.ToShortDateString());
            }
        }

        private void AddPatient_Btn(object sender, RoutedEventArgs e)
        {
            AddPatient addPatient = new();
            addPatient.ShowDialog();
        }
    }

    internal class User
    {
        public int Chartnum { get; set; }
        public int patientnum { get; set; }
        public string? Name { get; set; }
        public bool Sex { get; set; }
        public int Age { get; set; }
        public string? SecurityNum { get; set; }
        public int Birthday { get; set; }
        public string? RecentVisit { get; set; }
        public string? Address { get; set; }

    }
}
