using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.Models
{
	public class ProcedureModel : ObservableObject
	{
		private int         mediProcedureID = 0;		//시술번호
		private int         treatmentAmount = 0;		//단가
		private DateTime    createtionDate;				//생성날짜
		private DateTime    revisedDate;				//개정날짜
		private string?		afterS = null;              //사후관리
		private string?		procedure_Info = null;      //시술정보
		private string?		procedureName = null;		//시술이름


		public int MediProcedureID
		{
			get => mediProcedureID;
			set => SetProperty(ref mediProcedureID, value);
		}

		public int TreatmentAmount
		{
			get => treatmentAmount;
			set => SetProperty(ref treatmentAmount, value);
		}

		public DateTime CreatetionDate
		{
			get => createtionDate;
			set => SetProperty(ref createtionDate, value);
		}

		public DateTime RevisedDate
		{
			get => revisedDate;
			set => SetProperty(ref revisedDate, value);
		}

		public string? AfterS
		{
			get => afterS;
			set => SetProperty(ref afterS, value);
		}

		public string? ProcedureName
		{
			get => procedureName;
			set => SetProperty(ref procedureName, value);
		}

		public string? Procedure_Info
		{
			get => procedure_Info;
			set => SetProperty(ref procedure_Info, value);
		}

	}
}
