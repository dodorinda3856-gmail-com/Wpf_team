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
    /// AddDisease.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddDisease : Window
    {
        OracleConnection? connn;

        public AddDisease()
        {
            InitializeComponent();
            LogRecord.LogWrite("상병코드 추가창 열림");
        }

        //취소버튼
        private void cancel_add_Disease_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("상병코드 추가 취소 임시버튼 클릭");
            var result = MessageBox.Show("취소하시겠습니까?", "취소", MessageBoxButton.YesNo);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
            {
                LogRecord.LogWrite("상병코드 추가 취소 최종버튼 클릭");
                Close();
            }
               
        }

        //저장버튼
        private void add_Disease_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("상병코드 추가 저장 임시버튼 클릭");
            var result = MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                if (CheckRightValue())
                    return;

                ConnectDB();

                InsertSQL();
                LogRecord.LogWrite("상병코드 추가 저장 최종버튼 클릭");
                MessageBox.Show("저장되었습니다.");
                Close();
            }
        }

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

        //저장시 sql문
        private void InsertSQL()
        {
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = "INSERT INTO NAME_OF_DISEASE(DISEASE_ID" +
                                                    ", MEDI_PROCEDURE_ID" +
                                                    ", DISEASE_NAME" +
                                                    ", CREATION_DATE" +
                                                    ", REVISED_DATE" +
                                                    ", DELETE_OR_NOT" +
                                                    ", DISEASE_CODE" +
                                                    ", DISEASE_ENG" +
                                                    ", A_S) " +
                                "VALUES(DISEASE_SEQ.NEXTVAL" +
                                        ", 1" +
                                        ", '" + diseaseName_Kor_textBox.Text + "'" +
                                        ", To_Date('" + DateTime.Now.ToString("yyyy-MM-dd") + "', 'yyyy-MM-dd')" +
                                        ", To_Date('" + DateTime.Now.ToString("yyyy-MM-dd") + "', 'yyyy-MM-dd')" +
                                        ", 'T'" +
                                        ", '" + diseaseCode_textBox.Text + "'" +
                                        ", '" + diseaseName_Eng_textBox.Text + "'" +
                                        ", '" + diseaseAs_textBox.Text + "')";

            //실행시키는기능
            comm.ExecuteNonQuery();
            connn.Close();
        }

        private bool CheckRightValue()
        {
            if (diseaseCode_textBox.Text == "")
            {
                MessageBox.Show("상병코드를 입력해주세요.");
                return true;
            }
            else if (diseaseName_Kor_textBox.Text == "")
            {
                MessageBox.Show("상병 한글명을 입력해주세요");
                return true;
            }
            if (diseaseName_Eng_textBox.Text == "")
            {
                diseaseName_Eng_textBox.Text = "-";
            }
            if (diseaseAs_textBox.Text == "")
            {
                diseaseAs_textBox.Text = "-";
            }
            return false;
        }
    }
}
