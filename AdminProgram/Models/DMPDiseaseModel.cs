using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminProgram.Models
{
	public class DMPDiseaseModel : ObservableObject
	{
		private int disease_ID = 0;             //질병id
		private string? disease_Name = null;    //상병명
		private DateTime createtionDate;        //생성날짜
		private DateTime revisedDate;           //개정날짜
		private string? afterS = null;          //사후관리
		private string? disease_Code = null;    //상병코드
		private string? disease_ENG = null;		//상병명_영어

		public int DiseaseID
		{
			get => disease_ID;
			set => SetProperty(ref disease_ID, value);
		}

		public string? Disease_Name
		{
			get => disease_Name;
			set => SetProperty(ref disease_Name, value);
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

		public string? Disease_Code
		{
			get => disease_Code;
			set => SetProperty(ref disease_Code, value);
		}

		public string? Disease_ENG
		{
			get => disease_ENG;
			set => SetProperty(ref disease_ENG, value);
		}
	}
}
