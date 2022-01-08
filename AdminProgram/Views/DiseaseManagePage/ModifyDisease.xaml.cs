using AdminProgram.Models;
using Oracle.ManagedDataAccess.Client;
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
using System.Windows.Shapes;

namespace AdminProgram
{
    /// <summary>
    /// ModifyDisease.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifyDisease : Window
    {
        OracleConnection connn;

        private static DMPDiseaseModel s_value;
        public static DMPDiseaseModel Passvalue
        {
            get { return s_value; }
            set { s_value = value; }
        }

        public ModifyDisease()
        {
            InitializeComponent();
            diseaseCode_textBlock.Text = s_value.Disease_Code;
            diseaseName_Kor_textBlock.Text = s_value.Disease_Name;
            diseaseName_Eng_textBlock.Text = s_value.Disease_ENG;
            diseaseAs_textBox.Text = s_value.AfterS;
        }


        //DB 연결
        private void ConnectDB()
        {
            try
            {
                string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";
                connn = new OracleConnection(strCon);
                connn.Open();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        //수정버전 sql
        private void ModifiedInsertSQL()
        {
            OracleCommand comm = new();

            if (diseaseAs_textBox.Text == "")
                diseaseAs_textBox.Text = "-";

            comm.Connection = connn;
            comm.CommandText = "UPDATE NAME_OF_DISEASE " +
                                "SET A_S = '" + diseaseAs_textBox.Text + "' " +
                                "where DISEASE_NAME = '" + s_value.Disease_Name + "'";

            //실행시키는기능
            comm.ExecuteNonQuery();
            connn.Close();
        }

        //삭제버전 sql
        private void DeleteInsertSQL()
        {
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = "update NAME_OF_DISEASE " +
                                "set DELETE_OR_NOT = 'F' " +
                                "where DISEASE_NAME = '" + s_value.Disease_Name + "'";

            //실행시키는기능
            comm.ExecuteNonQuery();
            connn.Close();
        }

        private void delete_add_Disease_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("'" + s_value.Disease_Code + ", " + s_value.Disease_Name + "' 상병코드 정보 삭제 버튼 클릭");
            var result = MessageBox.Show("상병코드정보를 삭제하시겠습니까?", "삭제", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                LogRecord.LogWrite("'" + s_value.Disease_Code + ", "+ s_value.Disease_Name + "' 상병코드 정보 삭제 완료버튼 클릭");
                ConnectDB();

                DeleteInsertSQL();

                MessageBox.Show("해당 상병코드가 삭제되었습니다.");
                Close();
            }
            else
                LogRecord.LogWrite("'" + s_value.Disease_Code + ", " + s_value.Disease_Name + "' 상병코드 정보 삭제 취소 버튼 클릭");
        }

        private void modify_Disease_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("'" + s_value.Disease_Code + ", " + s_value.Disease_Name + "' 상병코드 정보 수정 버튼 클릭");
            var result = MessageBox.Show("위 내용으로 수정하시겠습니까?", "수정", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                LogRecord.LogWrite("'" + s_value.Disease_Code + ", " + s_value.Disease_Name + "' 상병코드 정보 수정 완료 버튼 클릭");
                ConnectDB();

                ModifiedInsertSQL();

                MessageBox.Show("수정되었습니다.");
                Close();
            }
            else
                LogRecord.LogWrite("'" + s_value.Disease_Code + ", "+ s_value.Disease_Name + "' 상병 정보 수정 취소 버튼 클릭");
        }
    }
}
