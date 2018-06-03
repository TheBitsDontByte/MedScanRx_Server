using System;
using System.Data.SqlClient;
using MedScanRx.Exceptions;
using MedScanRx.Models;

namespace MedScanRx.DAL.Mapper
{
    public static class DataRowToPatientMapper
    {
        public static Patient_Model Map(SqlDataReader reader)
        {
            try
            {
                return new Patient_Model
                {
                    PatientId = (int)reader["PatientId"],
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    DateOfBirth = (DateTime)reader["DateOfBirth"],
                    Gender = reader["Gender"].ToString(),
                    Phone1 = reader["Phone1"].ToString(),
                    Phone2 = reader["Phone2"].ToString(),
                    Email = reader["Email"].ToString(),
                    EmergencyContactName = reader["EmergencyContactName"].ToString(),
                    EmergencyContactRelation = reader["EmergencyContactRelation"].ToString(),
                    EmergencyContactPhone = reader["EmergencyContactPhone"].ToString(),
                    PreferredHospital = reader["PreferredHospital"].ToString(),
                    PreferredPhysician = reader["PreferredPhysician"].ToString(),
                };
            }
            catch (Exception ex)
            {
                throw new MappingException("Mapping a data row to a patient model failed", ex);
            }
        }
    }
}
