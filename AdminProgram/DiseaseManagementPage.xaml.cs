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
using System.Reflection;

namespace AdminProgram
{
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

		//상병 SQL------------------------------------------------------------------------------------
		private void MakeDiseaseSQL(ref string? sql)
		{
			sql = "select DISEASE_ID, DISEASE_NAME, CREATION_DATE, REVISED_DATE, A_S, DISEASE_CODE, DISEASE_ENG from NAME_OF_DISEASE " +
			"where DELETE_OR_NOT = 'T' and " +
			"DISEASE_CODE like '%" + diseaseNum_txtbox.Text + "%' and " +
			"DISEASE_NAME LIKE '%" + diseaseName_txtbox.Text + "%' " +
			"order by DISEASE_CODE";
		}

		//상병코드검색
		private void Search_Disease_Button_Click(object sender, RoutedEventArgs e)
		{
			LogRecord.LogWrite("상병 검색 버튼 클릭");
			string? sql = null;

			ConnectDB();
			MakeDiseaseSQL(ref sql);

			OracleCommand comm = new();

			comm.Connection = connn;
			comm.CommandText = sql;
			OracleDataReader reader = comm.ExecuteReader();
			List<DMPDiseaseModel> datas = new();

			while (reader.Read())
			{
				datas.Add(new DMPDiseaseModel()
				{
					Disease_Code = reader.GetString(reader.GetOrdinal("DISEASE_CODE")),
					Disease_Name = reader.GetString(reader.GetOrdinal("DISEASE_NAME")),
					Disease_ENG = reader.GetString(reader.GetOrdinal("DISEASE_ENG")),
					AfterS = reader.GetString(reader.GetOrdinal("A_S")),
					CreatetionDate = reader.GetDateTime(reader.GetOrdinal("CREATION_DATE"))

				});

			}

			diseaseDataGrid.ItemsSource = datas;

			reader.Close();
		}

		//상병상세정보
		private void Row_DiseaseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

		//상병추가버튼클릭
		private void Add_Disease_Button_Click(object sender, RoutedEventArgs e)
		{
			AddDisease adddisease = new();
			adddisease.ShowDialog();
		}

		//시술 SQL--------------------------------------------------------------------------------------
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
