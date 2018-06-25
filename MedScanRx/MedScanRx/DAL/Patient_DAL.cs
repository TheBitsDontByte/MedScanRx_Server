
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

        public async Task<bool> SavePatient(Patient_Model patient)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO Patient (FirstName, LastName, DateOfBirth, Gender, Phone1, Phone2, Email, EmergencyContactName, " +
                                                   "EmergencyContactRelation, EmergencyContactPhone, PreferredHospital, PreferredPhysician, " +
                                                   "IsActive, EnteredBy, EnteredDate, ModifiedBy, ModifiedDate) " +
                                      "output inserted.PatientId " +
                                      "VALUES(@FirstName, @LastName, @DateOfBirth, @Gender, @Phone1, @Phone2, @Email, @EmergencyContactName, @EmergencyContactRelation, " +
                                               "@EmergencyContactPhone, @PreferredHospital, @PreferredPhysician, @IsActive, @EnteredBy, @EnteredDate, @ModifiedBy, @ModifiedDate)"
                };

                DateTime now = DateTime.Now;

                cmd.Parameters.AddWithValue("@FirstName", patient.FirstName);
                cmd.Parameters.AddWithValue("@LastName", patient.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", patient.Gender);
                cmd.Parameters.AddWithValue("@Phone1", patient.Phone1);
                cmd.Parameters.AddWithValue("@Phone2", patient.Phone2 ?? "");
                cmd.Parameters.AddWithValue("@Email", patient.Email);
                cmd.Parameters.AddWithValue("@EmergencyContactName", patient.EmergencyContactName ?? "");
                cmd.Parameters.AddWithValue("@EmergencyContactRelation", patient.EmergencyContactRelation ?? "");
                cmd.Parameters.AddWithValue("@EmergencyContactPhone", patient.EmergencyContactPhone ?? "");
                cmd.Parameters.AddWithValue("@PreferredHospital", patient.PreferredHospital ?? "");
                cmd.Parameters.AddWithValue("@PreferredPhysician", patient.PreferredPhysician ?? "");
                cmd.Parameters.AddWithValue("@IsActive", 1);//New patient default to active
                cmd.Parameters.AddWithValue("@EnteredBy", "Whomst here ?");
                cmd.Parameters.AddWithValue("@EnteredDate", now);
                cmd.Parameters.AddWithValue("@ModifiedBy", "Whomst here ?");
                cmd.Parameters.AddWithValue("@ModifiedDate", now);

                await cn.OpenAsync().ConfigureAwait(false);
                var patientId = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                //Check that a valid patientId was generated
                if (patientId < 100000)
                    return false;

                patient.PatientId = patientId;
                return true;

            }
            catch (Exception ex)
            {
                //Log
                throw new DatabaseException($"Something went wrong saving the patient.", ex);

            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<bool> UpatePatient(Patient_Model patient)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "UPDATE [dbo].[Patient] SET[FirstName] = @FirstName, [LastName] = @LastName, " +
                            "[DateOfBirth] = @DateOfBirth, [Gender] = @Gender, [Phone1] = @Phone1, [Phone2] = @Phone2, [Email] = @Email," +
                            " [EmergencyContactName] = @EmergencyContactName, [EmergencyContactRelation] = @EmergencyContactRelation, " +
                            "[EmergencyContactPhone] = @EmergencyContactPhone, [PreferredHospital] = @PreferredHospital, " +
                            "[PreferredPhysician] = @PreferredPhysician, [ModifiedBy] = @ModifiedBy, [ModifiedDate] = @ModifiedDate " +
                            "WHERE PatientId = @PatientId "
                };

                cmd.Parameters.AddWithValue("@FirstName", patient.FirstName);
                cmd.Parameters.AddWithValue("@LastName", patient.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", patient.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", patient.Gender);
                cmd.Parameters.AddWithValue("@Phone1", patient.Phone1);
                cmd.Parameters.AddWithValue("@Phone2", patient.Phone2 ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", patient.Email);
                cmd.Parameters.AddWithValue("@EmergencyContactName", patient.EmergencyContactName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EmergencyContactRelation", patient.EmergencyContactRelation ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EmergencyContactPhone", patient.EmergencyContactPhone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PreferredHospital", patient.PreferredHospital ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PreferredPhysician", patient.PreferredPhysician ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedBy", "Whomst here ?");
                cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@PatientId", patient.PatientId);

                await cn.OpenAsync().ConfigureAwait(false);
                var success = (int)await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                //Check that a valid patientId was generated
                return success == 1;

            }
            catch (Exception ex)
            {
                //Log
                throw new DatabaseException($"Something went wrong saving the patient.", ex);

            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<bool> DeletePatient(long patientId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = $"UPDATE PATIENT SET IsActive = 0 WHERE PatientId = @PatientId"
                };

                cmd.Parameters.AddWithValue("@PatientId", patientId);

                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Something went wrong deleting the patient.", ex);
            }
        }

    }
}
