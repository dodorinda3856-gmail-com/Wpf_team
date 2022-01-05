using AdminProgram.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
	/// DiseaseManagementPage.xaml에 대한 상호 작용 논리
	/// </summary>
	
	
	
	///해야할것들 - 추가버튼클릭시 window 띄우기
	///           - 상세정보페이지 구성, 상세정보에서 수정,삭제버튼이벤트
	///           - 데이터 모델 넣고 가져오기
	///           - 상병코드 데이터 넣기
	




	public partial class DiseaseManagementPage : Page
	{
		OracleConnection connn;

		public DiseaseManagementPage()
		{
			InitializeComponent();
			LogRecord.LogWrite("상병/시술페이지 오픈");
		}

		//DB연결
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

		//상병 SQL
		/*private void MakeDiseaseSQL(ref string? sql)
		{
			if (gender_combobox.SelectedItem.ToString()[(gender_combobox.SelectedValue.ToString().Length - 1)..] == "-")
			{
				if (bod_txtbox.Text == "")
					sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT " +
					"where PATIENT_STATUS_VAL = 'T' and " +
					"PATIENT_ID like '%" + patientNum_txtbox.Text + "%' and " +
					"PATIENT_NAME LIKE '%" + patientName_txtbox.Text + "%' and " +
					"DOB BETWEEN To_Date('" + "18000101'" + ", 'yyyyMMDD') and To_Date('" + "20301231'" + ", 'yyyyMMDD') and " +
					"DOB BETWEEN To_Date('" + startyear + "0101'" + ", 'yyyyMMDD') and To_Date('" + endyear + "1231'" + ", 'yyyyMMDD') and " +
					"PHONE_NUM LIKE '%" + phoneNum_txtbox.Text + "%'" +
					" order by PATIENT_ID";

			}
			//sql = "select PATIENT_ID, RESIDENT_REGIST_NUM, ADDRESS, PATIENT_NAME, PHONE_NUM, REGIST_DATE, GENDER, DOB from PATIENT order by PATIENT_ID";
		}*/

		//상병코드검색--------------------------------------
		/*private void Search_Disease_Button_Click(object sender, RoutedEventArgs e)
		{
			LogRecord.LogWrite("상병 검색 버튼 클릭");
			string? sql = null;


			//if (CheckRightValue())
				return;

			ConnectDB();

			MakeDiseaseSQL(ref sql);

			OracleCommand comm = new();

			comm.Connection = connn;
			comm.CommandText = sql;
			OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
			List<DiseaseModel> datas = new();

			while (reader.Read())
			{
				datas.Add(new DiseaseModel()
				{
					Patient_ID = reader.GetInt32(reader.GetOrdinal("PATIENT_ID")),
					Resident_Regist_Num = reader.GetString(reader.GetOrdinal("Resident_Regist_Num")),
					Address = reader.GetString(reader.GetOrdinal("Address")),
					Patient_Name = reader.GetString(reader.GetOrdinal("Patient_Name")),
					Phone_Num = reader.GetString(reader.GetOrdinal("Phone_Num")),
					Regist_Date = reader.GetDateTime(reader.GetOrdinal("Regist_Date")),
					Gender = reader.GetString(reader.GetOrdinal("Gender")),
					Dob = reader.GetDateTime(reader.GetOrdinal("Dob")),
					Age = Calculate_age(reader.GetDateTime(reader.GetOrdinal("Dob")))
				});
			}
			diseaseDataGrid.ItemsSource = datas;

			reader.Close();

		}*/

		//상병상세정보
		private void Row_DiseaseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		//시술 SQL--------------------------------------
		private void MakeProcedureSQL(ref string? sql)
		{
			sql = "select MEDI_PROCEDURE_ID, TREATMENT_AMOUNT, CREATETION_DATE, REVISED_DATE, A_S, PROCEDURE_NAME, PROCEDURE_INFO from MEDI_PROCEDURE " +
			"where DELETE_OR_NOT = 'T' and " +
			"MEDI_PROCEDURE_ID like '%" + treatmentNum_txtbox.Text + "%' and " +
			"PROCEDURE_NAME LIKE '%" + treatmentName_txtbox.Text + "%' " +
			"order by MEDI_PROCEDURE_ID";
		}

		//시술코드검색
		private void Search_Procedure_Button_Click(object sender, RoutedEventArgs e)
		{
			LogRecord.LogWrite("시술 검색 버튼 클릭");
			string? sql = null;


			//if (CheckRightValue())

			ConnectDB();

			MakeProcedureSQL(ref sql);

			OracleCommand comm = new();

			comm.Connection = connn;
			comm.CommandText = sql;
			OracleDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
			List<ProcedureModel> datas = new();

			while (reader.Read())
			{
				datas.Add(new ProcedureModel()
				{
					MediProcedureID = reader.GetInt32(reader.GetOrdinal("MEDI_PROCEDURE_ID")),
					TreatmentAmount = reader.GetInt32(reader.GetOrdinal("TREATMENT_AMOUNT")),
					CreatetionDate = reader.GetDateTime(reader.GetOrdinal("CREATETION_DATE")),
					AfterS = reader.GetString(reader.GetOrdinal("A_S")),
					ProcedureName = reader.GetString(reader.GetOrdinal("PROCEDURE_NAME")),
					Procedure_Info = reader.GetString(reader.GetOrdinal("PROCEDURE_INFO"))
				});
			}
			treatmentDataGrid.ItemsSource = datas;

			reader.Close();
		}

		//시술상세정보
		private void Row_TreatmentDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var row = sender as DataGridRow;

			if (row != null && row.IsSelected)
			{
				ProcedureModel tmp = (ProcedureModel)row.Item;
				ModifyProcedure.PassedProcedure = tmp;
				ModifyProcedure tw = new ModifyProcedure();

				tw.ShowDialog();
			}
		}

		//시술추가버튼클릭
		private void Add_Procedure_Button_Click(object sender, RoutedEventArgs e)
		{
			AddProcedure addprocedure = new();
			addprocedure.ShowDialog();
		}
	}
}
