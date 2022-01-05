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
    /// AddProcedure.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddProcedure : Window
    {
        OracleConnection connn;

        public AddProcedure()
        {
            InitializeComponent();
        }
        

        //숫자만 있는지 확인
        private bool checkOnlyNum(string s)
        {
            for(int i = 0; i < s.Length; i++)
            {
                if(s[i] >= '0' && s[i] <= '9')
                    return true;
            }
            return false;
        }

        //올바른 값이 들어왔는지 확인
        private bool CheckRightValue()
        {
            if (procedureName_textBox.Text == "")
            {
                MessageBox.Show("시술명을 입력해주세요.");
                return true;
            }
            else if (treatment_textbox.Text == "" || !checkOnlyNum(treatment_textbox.Text))
            {
                MessageBox.Show("단가를 숫자로만 입력해주세요\n(단위 : 원)");
                return true;
            }
            else if (procedure_textbox.Text == "")
            {
                MessageBox.Show("시술정보를 입력해주세요.");
                return true;
            }
            else if (as_textbox.Text == "")
            {
                as_textbox.Text = "-";
            }
            return false;
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
            comm.CommandText = "INSERT INTO MEDI_PROCEDURE(MEDI_PROCEDURE_ID" +
                                                    ", TREATMENT_AMOUNT" +
                                                    ", CREATETION_DATE" +
                                                    ", REVISED_DATE" +
                                                    ", DELETE_OR_NOT" +
                                                    ", A_S" +
                                                    ", PROCEDURE_NAME) " +
                                "VALUES(MEDI_PRO_SEQ.NEXTVAL" +
                                        ", '" + treatment_textbox.Text + "'" +
                                        ", To_Date('" + DateTime.Now.ToString("yyyy-MM-dd") + "', 'yyyy-MM-dd')" +
                                        ", To_Date('" + DateTime.Now.ToString("yyyy-MM-dd") + "', 'yyyy-MM-dd')" +
                                        ", 'T'" +
                                        ", '" + as_textbox.Text + "'" +
                                        ", '" + procedureName_textBox.Text + "')";

            //실행시키는기능
            comm.ExecuteNonQuery();
            connn.Close();
        }

        //저장버튼
        private void add_Procedure_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                if (CheckRightValue())
                    return;

                ConnectDB();

                InsertSQL();

                MessageBox.Show("저장되었습니다.");
                Close();
            }
        }

        //취소버튼
        private void cancel_add_Procedure_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("취소하시겠습니까?", "취소", MessageBoxButton.YesNo);

            // If the no button was pressed ...
            if (result == MessageBoxResult.Yes)
                Close();
        }
    }
}
