
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MedScanRx.DAL.Mapper;
using MedScanRx.Exceptions;
using MedScanRx.Models;

namespace MedScanRx.DAL
{
    public class Patient_DAL
    {
        //Grab this from appsettings.json instead of here
        
        private static SqlConnection cn = new SqlConnection("Server=DESKTOP-CLVNC1I;Database=MedScanRx;User Id=admin;Password=admin");

        public async Task<List<Patient_Model>> GetAllPatients()
        { 
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = System.Data.CommandType.Text,
                CommandText = "select * from dbo.Patient",
            };

            List<Patient_Model> allPatients = new List<Patient_Model>();

            try
            {
                
                await cn.OpenAsync().ConfigureAwait(false);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allPatients.Add(DataRowToPatientMapper.Map(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                //Log error 
                throw new DatabaseException("Something went wrong getting all patients.", ex);
            }
            finally
            {
                cn.Close();
            }

            return allPatients;
        }

        public async Task<Patient_Model> GetPatient(long patientId)
        {
            SqlCommand cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = System.Data.CommandType.Text,
                CommandText = "select * from dbo.Patient where PatientId = @patientId",
            };

            Patient_Model patient = null;

            try
            {
                cmd.Parameters.AddWithValue("@patientId", patientId);
                await cn.OpenAsync().ConfigureAwait(false);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        patient = DataRowToPatientMapper.Map(reader);   
                    //Do i want a check here for more than one row ? I know in the db I've set it up where it just cant happen ... 
                    //Leave as is for now
                }
            }
            catch (Exception ex)
            {
                //Log
                throw new DatabaseException($"Something went wrong getting the patient with id {patientId}.", ex);

            }
            finally
            {
                cn.Close();
            }

            return patient;
        }

    }
}
