using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminProgram.Models;

namespace AdminProgram.ViewModels
{
    public class SPViewModel
    {
        private readonly ILogger _logger;
        string strCon = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=loonshot.cgxkzseoyswk.us-east-2.rds.amazonaws.com)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));User Id=loonshot;Password=loonshot123;";


        public List<TMTModel> PieChartStart()
        {
            List<TMTModel> treatCount = new();

            string sql = "SELECT tc.TREAT_TYPE , count(*) AS TREAT_COUNT " +
                "FROM(" +
                "(SELECT r.PATIENT_ID, r.TREAT_TYPE " +
                "FROM RESERVATION r LEFT JOIN PATIENT p ON p.PATIENT_ID = r.PATIENT_ID " +
                "WHERE p.PATIENT_STATUS_VAL = 'T'  AND r.TREAT_TYPE IS NOT NULL AND TO_CHAR(r.RESERVATION_DATE, 'YYYY-MM') BETWEEN TO_CHAR(SYSDATE, 'YYYY-MM') AND TO_CHAR(SYSDATE, 'YYYY-MM')) " +
                "UNION ALL " +
                "(SELECT w.PATIENT_ID, w.TREAT_TYPE " +
                "FROM WAITING w LEFT JOIN PATIENT p ON p.PATIENT_ID = w.PATIENT_ID " +
                "WHERE p.PATIENT_STATUS_VAL = 'T' AND w.TREAT_TYPE IS NOT NULL AND TO_CHAR(w.REQUEST_TO_WAIT, 'YYYY-MM') BETWEEN TO_CHAR(SYSDATE, 'YYYY-MM') AND TO_CHAR(SYSDATE, 'YYYY-MM') ) ) tc " +
                "GROUP BY tc.TREAT_TYPE";


            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {

                            try
                            {
                                while (reader.Read())
                                {
                                    treatCount.Add(new TMTModel()
                                    {
                                        TREAT_TYPE = reader.GetString(reader.GetOrdinal("TREAT_TYPE")),
                                        TREAT_COUNT = reader.GetInt32(reader.GetOrdinal("TREAT_COUNT"))
                                    });
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    _logger.LogInformation(e.ToString());
                }
            }
            return treatCount;
        }

        public List<int> LiveChartStart() {

            List<int> monthCount = new();

            string sql = "SELECT count(t.d) AS month_total " +
                "FROM(SELECT TO_CHAR(w.REQUEST_TO_WAIT, 'YYYY-MM') AS d " +
                "FROM WAITING w " +
                "WHERE TO_CHAR(w.REQUEST_TO_WAIT, 'YYYY') BETWEEN TO_CHAR(SYSDATE, 'YYYY') AND TO_CHAR(SYSDATE, 'YYYY') " +
                "UNION ALL " +
                "SELECT TO_CHAR(r.RESERVATION_DATE, 'YYYY-MM') AS d " +
                "FROM RESERVATION r " +
                "WHERE TO_CHAR(r.RESERVATION_DATE, 'YYYY') BETWEEN TO_CHAR(SYSDATE, 'YYYY') AND TO_CHAR(SYSDATE, 'YYYY')) t , " +
                "(SELECT TO_CHAR(SYSDATE, 'YYYY') || '-' || LPAD(LEVEL, 2, '0') LV " +
                "FROM DUAL CONNECT BY LEVEL <= 12) b " +
                "WHERE b.LV = t.d(+) " +
                "GROUP BY b.LV " +
                "ORDER BY b.LV";

            using (OracleConnection conn = new OracleConnection(strCon))
            {
                try
                {
                    conn.Open();

                    using (OracleCommand comm = new OracleCommand())
                    {
                        comm.Connection = conn;
                        comm.CommandText = sql;

                        using (OracleDataReader reader = comm.ExecuteReader())
                        {

                            try
                            {
                                while (reader.Read())
                                {
                                    monthCount.Add((reader.GetInt16(reader.GetOrdinal("month_total"))));
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }
                    }
                }

                catch (Exception e)
                {
                    _logger.LogInformation(e.ToString());
                }
            }
            return monthCount;
        }
    }
}