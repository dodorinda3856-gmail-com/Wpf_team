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
    /// ModifyProcedure.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifyProcedure : Window
    {
        OracleConnection connn;

        private static ProcedureModel passedProcedure;
        public static ProcedureModel PassedProcedure
        {
            get { return passedProcedure; }
            set { passedProcedure = value; }
        }

        public ModifyProcedure()
        {
            LogRecord.LogWrite("'" + passedProcedure.ProcedureName + "' 시술 상세 정보창 열림");
            InitializeComponent();
            procedureName_textBox.Text = passedProcedure.ProcedureName;
            treatment_textbox.Text = passedProcedure.TreatmentAmount.ToString();
            procedure_textbox.Text = passedProcedure.Procedure_Info;
            as_textbox.Text = passedProcedure.AfterS;
            create_Date.Text = passedProcedure.CreatetionDate.ToString().Substring(0,10);
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

        //수정SQL
        private void ModifiedInsertSQL()
        {
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = "UPDATE MEDI_PROCEDURE " +
                                "SET PROCEDURE_NAME = '" + procedureName_textBox.Text + "'" +
                                  ", TREATMENT_AMOUNT = '" + treatment_textbox.Text + "'" +
                                  ", PROCEDURE_INFO = '" + procedure_textbox.Text + "'" +
                                  ", REVISED_DATE = " + "To_Date('" + DateTime.Now.ToString("yyyy-MM-dd") + "', 'yyyy-MM-dd')" +
                                  ", A_S = '" + as_textbox.Text + "' " +
                                "where MEDI_PROCEDURE_ID = " + passedProcedure.MediProcedureID;

            comm.ExecuteNonQuery();
            connn.Close();
        }

        //수정버튼 클릭이벤트
        private void modify_Procedure_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("'" + procedureName_textBox.Text + "' 시술 정보 수정 임시버튼 클릭");
            var result = MessageBox.Show("위 내용으로 수정하시겠습니까?", "수정", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                LogRecord.LogWrite("'" + procedureName_textBox.Text + "' 시술 정보 수정 최종버튼 클릭");
                ConnectDB();

                ModifiedInsertSQL();

                MessageBox.Show("수정되었습니다.");
                Close();
            }
            else
                LogRecord.LogWrite("'" + procedureName_textBox.Text + "' 시술 정보 수정 취소 버튼 클릭");
        }

        //삭제SQL
        private void DeleteInsertSQL()
        {
            OracleCommand comm = new();

            comm.Connection = connn;
            comm.CommandText = "update MEDI_PROCEDURE " +
                                "set DELETE_OR_NOT = 'F' " +
                                "where MEDI_PROCEDURE_ID = " + passedProcedure.MediProcedureID;

            comm.ExecuteNonQuery();
            connn.Close();
        }

        //삭제버튼 클릭이벤트
        private void delete_Procedure_Click(object sender, RoutedEventArgs e)
        {
            LogRecord.LogWrite("'" + procedureName_textBox.Text + "' 시술 정보 삭제 임시버튼 클릭");
            var result = MessageBox.Show("시술정보를 삭제하시겠습니까?", "삭제", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                LogRecord.LogWrite("'" + procedureName_textBox.Text + "' 시술 정보 삭제 최종버튼 클릭");
                ConnectDB();

                DeleteInsertSQL();

                MessageBox.Show("해당 시술이 삭제되었습니다.");
                Close();
            }
            else
                LogRecord.LogWrite("'" + procedureName_textBox.Text + "' 시술 정보 삭제 취소 버튼 클릭");
        }
    }
}
