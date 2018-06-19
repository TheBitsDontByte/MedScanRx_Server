using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.DAL.Mapper;
using MedScanRx.Exceptions;
using MedScanRx.Models;

namespace MedScanRx.DAL
{
    public class AppPrescription_DAL
    {
        private static SqlConnection cn = new SqlConnection("Server=DESKTOP-CLVNC1I;Database=MedScanRx;User Id=admin;Password=admin");

        public async Task<List<Prescription_Model>> GetUpcomingAlerts(long patientId)
        {
            List<Prescription_Model> allUpcomingPrescriptions = new List<Prescription_Model>();

            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    Connection = cn,
                    CommandText = "SELECT p.*, min(pa.AlertDateTime) as NextAlert FROM Prescription p " +
                                    "JOIN PrescriptionAlert pa on pa.PrescriptionId = p.PrescriptionId " +
                                    "WHERE pa.AlertDateTime BETWEEN GETDATE() AND DATEADD(HOUR, 1, GETDATE()) AND p.PatientId = @PatientId AND pa.IsActive = 1 AND p.IsActive = 1 " +
                                    "group by p.PrescriptionId,Ndc,BrandName,GenericName,PatientId,Barcode,Color,Dosage,Identifier,Shape,DoctorNote, " +
                                    "Warning,OriginalNumberOfDoses,CurrentNumberOfDoses,OriginalNumberOfRefills,CurrentNumberOfRefills,p.IsActive, " +
                                    "EnteredBy,EnteredDate,ModifiedBy,ModifiedDate"
                };

                cmd.Parameters.AddWithValue("@patientId", patientId);

                await cn.OpenAsync().ConfigureAwait(false);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allUpcomingPrescriptions.Add(DataRowToAllPrescriptionsMapper.Map(reader));
                    }
                }

                return allUpcomingPrescriptions;

            }
            catch (Exception ex)
            {
                throw new DatabaseException("Something went wrong getting the upcoming alerts", ex);
            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<List<Prescription_Model>> GetAllPrescriptions(long patientId)
        {
            List<Prescription_Model> allUpcomingPrescriptions = new List<Prescription_Model>();

            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    Connection = cn,
                    CommandText = "SELECT p.*, min(pa.AlertDateTime) as NextAlert FROM Prescription p " +
                                    "JOIN PrescriptionAlert pa on pa.PrescriptionId = p.PrescriptionId " +
                                    "WHERE p.PatientId = @PatientId AND pa.IsActive = 1 AND p.IsActive = 1 " +
                                    "group by p.PrescriptionId,Ndc,BrandName,GenericName,PatientId,Barcode,Color,Dosage,Identifier,Shape,DoctorNote, " +
                                    "Warning,OriginalNumberOfDoses,CurrentNumberOfDoses,OriginalNumberOfRefills,CurrentNumberOfRefills,p.IsActive, " +
                                    "EnteredBy,EnteredDate,ModifiedBy,ModifiedDate"
                };

                cmd.Parameters.AddWithValue("@patientId", patientId);

                await cn.OpenAsync().ConfigureAwait(false);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allUpcomingPrescriptions.Add(DataRowToAllPrescriptionsMapper.Map(reader));
                    }
                }

                return allUpcomingPrescriptions;

            }
            catch (Exception ex)
            {
                throw new DatabaseException("Something went wrong getting the upcoming alerts", ex);
            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<Prescription_Model> GetPrescription(int prescriptionId)
        {
            Prescription_Model model = new Prescription_Model();

            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = $"SELECT * FROM Prescription WHERE PrescriptionId = @prescriptionId"
                };
                cmd.Parameters.AddWithValue("@prescriptionId", prescriptionId);

                await cn.OpenAsync().ConfigureAwait(false);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = DataRowToPrescriptionDetailMapper.Map(reader);
                    }
                }
                return model;

            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Something went wrong getting the prescription info for prescriptionId {prescriptionId}", ex);
            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<List<string>> GetPrescriptionAlerts(int prescriptionId)
        {
            List<string> alerts = new List<string>();

            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = $"SELECT AlertDateTime FROM PrescriptionAlert WHERE PrescriptionId = @prescriptionId"
                };
                cmd.Parameters.AddWithValue("@prescriptionId", prescriptionId);

                await cn.OpenAsync().ConfigureAwait(false);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        alerts.Add(DataRowToPrescriptionAlertsMapper.Map(reader));
                    }
                }
                return alerts;

            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Something went wrong getting the prescription info for prescriptionId {prescriptionId}", ex);
            }
            finally
            {
                cn.Close();
            }
        }


    }
}
