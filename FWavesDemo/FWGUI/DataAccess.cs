using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Dapper;
using Microsoft.Win32;
using WFGUI;
using WPFPageSwitch;

namespace WPFUI
{
    /// <summary>  
    ///  This class provides access to the stored procedures of fwaves Database. 
    /// </summary>  
    public class DataAccess
    {
        public List<Patient> GetPerson(long amka)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                //await connection.OpenAsync();
                return connection.Query<Patient>("dbo.Patients_GetByAM @AMKA", new { AMKA = amka }).ToList();
                //$"select * from People where LastName='{lastName}'" - SQL injection threat

            }
        }

        public List<MeasurementID> GetMeasurementIDs(long amka) //returns Date,PK from Measurements IDs
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                //await connection.OpenAsync();
                return connection.Query<MeasurementID>("dbo.GetMeasurementID @AMKA", new { AMKA = amka }).ToList();
            }
        }

        public List<DiagnosisHistogram> GetDiagnosisHistogram(int diagnosis) //returns Histogram data for a diagnosis
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                //await connection.OpenAsync();
                return connection.Query<DiagnosisHistogram>("dbo.GetDiagnosisPercentage @Diagnosis", new { Diagnosis = diagnosis }).ToList();
            }
        }

        public List<Patient> GetPatients() //returns Date,PK from Measurements IDs
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                //await connection.OpenAsync();
                return connection.Query<Patient>("dbo.GetPatients").ToList();
            }
        }
        public List<Measurement> GetMeasurements(long PK) // returns the 40 first measurements where WAVEPK >= (@FIRSTPK)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                //await connection.OpenAsync();
                return connection.Query<Measurement>("dbo.GetMeasurement @FIRSTPK", new { firstPK = PK }).ToList();         
            }
        }

        public void InsertPerson(long amka, long birth, int sex, float height, float armLength, float legLength, int diagnosis, string comment)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                connection.OpenAsync();
                var person = new List<Patient>();
                
                person.Add(new Patient { AMKA = amka,Birth = birth, Sex = sex, Height = height, ArmLength = armLength, LegLength = legLength, Diagnosis = diagnosis, Comment = comment });

                try
                {
                    connection.Execute("dbo.Patient_Insert @AMKA, @Birth, @Sex, @Height, @ArmLength, @Leglength, @Diagnosis, @Comment", person);
                }
                //$"select * from People where LastName='{lastName}'" - SQL injection threat
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Data has not been Imported due to :{0}", ex.Message), "Not Imported");
                }
            }
        }

        public void InsertPerson(long amka, int sex, float height, float armLength, float legLength, string comment)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                connection.OpenAsync();
                var person = new List<Patient>();

                person.Add(new Patient { AMKA = amka, Sex = sex, Height = height, ArmLength = armLength, LegLength = legLength, Comment = comment});

                try
                {
                    connection.Execute("dbo.Patient_Insert @AMKA, @Sex, @Height, @ArmLength, @Leglength, @Comment", person);
                }
                //$"select * from People where LastName='{lastName}'" - SQL injection threat
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Patient has not been Imported due to :{0}", ex.Message), "Not Imported");
                }
            }
        }

        public void EditPerson(long amka,int diagnosis, string comment)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                connection.OpenAsync();
                var person = new List<Patient>();

                person.Add(new Patient { AMKA = amka, Diagnosis = diagnosis, Comment = comment });

                try
                {
                    connection.Execute("dbo.EditPatient @AMKA, @Diagnosis, @Comment", person);
                }
                //$"select * from People where LastName='{lastName}'" - SQL injection threat
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Patient has not been Updated due to :{0}", ex.Message), "Not Updated");
                }
            }
        }

        public int DeletePerson(long amka)
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                connection.OpenAsync();
                try
                {
                    connection.Execute("dbo.DeletePatient @AMKA", new { AMKA = amka });
                    return 1;
                    
                }
                //$"select * from People where LastName='{lastName}'" - SQL injection threat
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Patient has not been deleted due to :{0}", ex.Message), "Not Deleted");
                    return 0;

                }
            }
        }

        public int DeleteMeasurement(long amka, long pk)
        {
            var measurementid = new List<MeasurementID>();

            measurementid.Add(new MeasurementID { AMKA = amka, PK = pk});
            using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                connection.OpenAsync();
                try
                {
                    connection.Execute("dbo.DeleteMeasurement @AMKA, @PK", measurementid);
                    return 1;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Patient has not been deleted due to :{0}", ex.Message), "Not Deleted");
                    return 0;

                }
            }
        }
        /// <summary>
        /// <c>InsertExcel</c> inserts an Excel study file into dbo.Measurements and dbo.MeasurementsID.
        /// </summary>
        public void InsertExcel(long amka, float limpLength, string date, string fileName, int neuron, int side, float mLat, float marea = 0, float mamp = 0, float fper = 0, float fRepPer = 0, int rNs=0, int rN5rep=0, int fRepsTotal=0 )
        {
            /********** Insert Excel File to dbo.Measurements *************/
            
            // Create Connection to Excel Workbook
            try
            {
                string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", fileName);
                System.Data.DataTable dt = null;

                OleDbConnection Econ = new OleDbConnection(constr);

                Econ.Open();
                dt = Econ.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }
                string Query = string.Format("Select * FROM [{0}]", excelSheets[0]); //query for what to select from excel sheet

                Econ.Close();
      
                Econ.Open();

                DataSet ds = new DataSet();
                OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
                Econ.Close();
                oda.Fill(ds);
                DataTable Exceldt = ds.Tables[0];

                //creating object of SqlBulkCopy      
                var objbulk = new SqlBulkCopy(Helper.CnnVal("fwavesDB"));
                //assigning Destination table name      
                objbulk.DestinationTableName = "dbo.Measurements";
                //Mapping Table column  
                string[] columnNames = Exceldt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                if (columnNames.Length >= 5)
                {
                    objbulk.ColumnMappings.Add(columnNames[0], "WAVEID");
                    objbulk.ColumnMappings.Add(columnNames[1], "LATENCY");
                    objbulk.ColumnMappings.Add(columnNames[2], "DURATION");
                    objbulk.ColumnMappings.Add(columnNames[3], "AMPLITUDE");
                    objbulk.ColumnMappings.Add(columnNames[4], "AREA");

                }
                else if (columnNames.Length == 4)
                {
                    objbulk.ColumnMappings.Add(columnNames[0], "WAVEID");
                    objbulk.ColumnMappings.Add(columnNames[1], "LATENCY");
                    objbulk.ColumnMappings.Add(columnNames[3], "AMPLITUDE");
                    objbulk.ColumnMappings.Add(columnNames[4], "AREA");
                }

                /* Computation of FCV */

                object minLatObject; // object that contains the FLatMin
                minLatObject = Exceldt.Compute("Min("+columnNames[1]+")", string.Empty);
                float fcvMax = 2 * limpLength * 10 / (Convert.ToSingle(minLatObject) - mLat  - 1); // limpLength * 10 to convert it to mm
                float numerator = fcvMax * (Convert.ToSingle(minLatObject) - 1);
                Exceldt.Columns.Add("FCV", typeof(float));
                int notUsedRows = 0;

                foreach (DataRow row in Exceldt.Rows)
                {
                    if (row[columnNames[0]] == DBNull.Value || row[columnNames[1]] == DBNull.Value) //check if WaveID or LATENCY columns are empty
                    {
                        row.Delete(); // if a row is empty, delete it from the DataTable
                        notUsedRows++;// unfortunately, even if you delete the row Exceldt.Rows.Count still counts this row
                    }
                    else if (row[columnNames[1]] != DBNull.Value) //check Latency column
                    {
                        row["FCV"] = numerator / (Convert.ToSingle(row[columnNames[1]]) - 1);   // computation of FCV for that row
                    }
                }

                objbulk.ColumnMappings.Add("FCV", "FCV");

                /* Insert Datatable Records to DataBase */  
                try
                {
                    SqlConnection sqlConnection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB"));
                    objbulk.WriteToServer(Exceldt);
                    objbulk.Close();

                    /********** Insert Data to dbo.MeasurementsID *************/
                    using (var connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
                    {
                        long lastPK;
                        connection.OpenAsync();
                        lastPK = (connection.Query<long>("dbo.GetLastPK", CommandType.StoredProcedure).First());
                        lastPK = lastPK + 1 - Exceldt.Rows.Count + notUsedRows;// Exceldt.Rows.Count contains the empty rows as well
                        fileName = System.IO.Path.GetFileName(fileName);
                        //$"select * from People where LastName='{lastName}'" - SQL injection threat

                        List<AddMeasurement> measurement = new List<AddMeasurement>();                    
                        measurement.Add(new AddMeasurement { AMKA = amka, Date = date, FileName = fileName, PK = lastPK, Neuron = neuron, Side = side, Mlat = mLat, Marea = marea, Mamp = mamp, Fper = fper, FRepPer = fRepPer , RNs = rNs, RN5Rep = rN5rep, FRepsTotal = fRepsTotal});
                        try
                        {
                            connection.Execute("dbo.MeasurementsInsert @AMKA, @DATE, @FILENAME, @PK, @Neuron, @Side, @Mlat, @Marea, @Mamp, @Fper, @FRepPer, @RNs, @RN5Rep, @FRepsTotal", measurement);
                        }
                        //$"select * from People where LastName='{lastName}'" - SQL injection threat
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Data has not been Imported due to: {0}", ex.Message), "Not Imported");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Data has not been Imported due to: {0}", ex.Message), "Not Imported");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Data has not been Imported due to: {0}", ex.Message), "Not Imported");
            }
        }

        
        /// <summary>
        /// <c>Prediction</c> Returns the diagnosis prediction of a measurement.
        /// </summary>
        public int Prediction(long amka)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                int pred;
                pred = 1;/*connection.Query<int> ("dbo.DiagnosisAMKAPK @AMKA, @PK", new { AMKA = 15000, PK = 1452}).First()*/
                return pred;
            }

        }

        
        /// <summary>
        /// <c>GetTotal</c> Returns number of Patients in Database.
        /// </summary>
        public int GetTotal()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("fwavesDB")))
            {
                //await connection.OpenAsync();
                int totalPersons = 0;
                try
                {
                    totalPersons = connection.Query<int>("dbo.GeneralStats", CommandType.StoredProcedure).First();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Cannot connect to the Server: {0}", ex.Message), "Problem with the Server Connectivity");
                }

                return totalPersons;
                //$"select * from People where LastName='{lastName}'" - SQL injection threat
            }
        }


    }
}
