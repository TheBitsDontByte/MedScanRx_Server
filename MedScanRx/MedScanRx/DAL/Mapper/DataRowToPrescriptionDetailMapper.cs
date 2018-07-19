using System;
using System.Data.SqlClient;
using MedScanRx.Exceptions;
using MedScanRx.Models;

namespace MedScanRx.DAL.Mapper
{
    public static class DataRowToPrescriptionDetailMapper
    {
        public static Prescription_Model Map(SqlDataReader reader)
        {
            try
            {
                return new Prescription_Model
                {
                    ImageUrl = reader["ImageUrl"].ToString(),
                    Rxcui = reader["Rxcui"].ToString(),
                    RximageMedicineName = reader["RximageMedicineName"].ToString(),
                    PrescriptionName = reader["PrescriptionName"].ToString(),
                    BrandName = reader["BrandName"].ToString(),
                    Color = reader["Color"].ToString(),
                    CurrentNumberOfDoses = reader["CurrentNumberOfDoses"].ToString(),
                    CurrentNumberOfRefills = reader["CurrentNumberOfRefills"].ToString(),
                    DoctorNotes = reader["DoctorNote"].ToString(),
                    Dosage = reader["Dosage"].ToString(),
                    GenericName = reader["GenericName"].ToString(),
                    Identifiers = reader["Identifier"].ToString(),
                    IsActive = reader["IsActive"].ToString(),
                    Ndc = reader["Ndc"].ToString(),
                    OriginalNumberOfDoses = reader["OriginalNumberOfDoses"].ToString(),
                    OriginalNumberOfRefills = reader["OriginalNumberOfRefills"].ToString(),
                    PatientId = (int)reader["PatientId"],
                    PrescriptionId = (int)reader["PrescriptionId"],
                    Shape = reader["Shape"].ToString(),
                    Warnings = reader["Warning"].ToString(),

                };
            }

            catch (Exception ex)
            {
                throw new MappingException("Mapping a data row to a prescription model failed", ex);
            }
        }
    }
}
