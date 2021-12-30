﻿using System;
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
using System.Windows.Shapes;

namespace AdminProgram.Views
{
    /// <summary>
    /// AppointmentDetailWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AppointmentDetailWindow : Window
    {
        public AppointmentDetailWindow()
        {
            InitializeComponent();
        }

        private void Delete_Reservation(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("예약 정보를 삭제했습니다");
            Window.GetWindow(this).Close();
        }
    }
}