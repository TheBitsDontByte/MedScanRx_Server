using System;
using System.Collections;
using MedScanRx.Models;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.Exceptions;
using System.Collections.Generic;
using MedScanRx.DAL.Mapper;

namespace MedScanRx.DAL
{
    public class Prescription_DAL
    {
        //There should be a DAL_Base class with this
        //The string should be in the config file
        private static SqlConnection cn = new SqlConnection("Server=DESKTOP-CLVNC1I;Database=MedScanRx;User Id=admin;Password=admin");


        public async Task<int> SavePrescription(Prescription_Model model)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = "INSERT INTO [dbo].[Prescription] " +
                                        "([Ndc], [BrandName], [GenericName],[PatientId],[Barcode],[Color],[Dosage],[Identifier]," +
                                        "[Shape],[DoctorNote],[Warning],[OriginalNumberOfDoses],[CurrentNumberOfDoses]," +
                                        "[OriginalNumberOfRefills],[CurrentNumberOfRefills],[IsActive],[EnteredBy]," +
                                        "[EnteredDate],[ModifiedBy],[ModifiedDate])" +
                                        "OUTPUT inserted.PrescriptionId " +
                                        "VALUES(@Ndc, @BrandName, @GenericName, @PatientId, @Barcode, @Color, @Dosage, @Identifier, " +
                                        "@Shape, @DoctorNote, @Warning, @OriginalNumberOfDoses, @CurrentNumberOfDoses, @OriginalNumberOfRefills, " +
                                        "@CurrentNumberOfRefills, @IsActive, @EnteredBy, @EnteredDate, @ModifiedBy, @ModifiedDate)"
                };

                DateTime now = DateTime.Now;

                cmd.Parameters.AddWithValue("@Ndc", model.Ndc);
                cmd.Parameters.AddWithValue("@BrandName", model.BrandName);
                cmd.Parameters.AddWithValue("@GenericName", model.GenericName);
                cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
                cmd.Parameters.AddWithValue("@Barcode", model.Barcode);
                cmd.Parameters.AddWithValue("@Color", model.Color);
                cmd.Parameters.AddWithValue("@Dosage", model.Dosage);
                cmd.Parameters.AddWithValue("@Identifier", model.Identifiers);
                cmd.Parameters.AddWithValue("@Shape", model.Shape);
                cmd.Parameters.AddWithValue("@DoctorNote", model.DoctorNotes);
                cmd.Parameters.AddWithValue("@Warning", model.Warnings);
                cmd.Parameters.AddWithValue("@OriginalNumberOfDoses", model.OriginalNumberOfDoses);
                cmd.Parameters.AddWithValue("@CurrentNumberOfDoses", model.OriginalNumberOfDoses);
                cmd.Parameters.AddWithValue("@OriginalNumberOfRefills", model.OriginalNumberOfRefills);
                cmd.Parameters.AddWithValue("@CurrentNumberOfRefills", model.OriginalNumberOfRefills);
                cmd.Parameters.AddWithValue("@IsActive", 1);
                cmd.Parameters.AddWithValue("@EnteredBy", "Where do I get user info?");
                cmd.Parameters.AddWithValue("@EnteredDate", now);
                cmd.Parameters.AddWithValue("@ModifiedBy", "SomeDumbUser");
                cmd.Parameters.AddWithValue("@ModifiedDate", now);

                await cn.OpenAsync().ConfigureAwait(false);
                return (int) await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Something went wrong saving the patient");
            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<bool> SavePrescriptionAlerts(Prescription_Model model)
        {
            try
            {

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    Connection = cn
                };


                string cmdString = "INSERT INTO PrescriptionAlert (PrescriptionId, AlertDateTime) VALUES ";
                cmd.Parameters.AddWithValue("@PrescriptionId", model.PrescriptionId);
                for(int i = 0; i < model.ScheduledAlerts.Count; i++)
                {
                    cmdString += $"(@PrescriptionId, @ScheduledAlert{i}),";
                    cmd.Parameters.AddWithValue($"@ScheduledAlert{i}", model.ScheduledAlerts.ElementAt(i));
                }
                cmd.CommandText = cmdString.Remove(cmdString.LastIndexOf(","), 1);

                await cn.OpenAsync().ConfigureAwait(false);
                var numOfInserts = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                if (numOfInserts != model.ScheduledAlerts.Count)
                    throw new Exception("Could not insert alerts for previously inserted prescription");

                return true;
            }
            catch (Exception ex)
            {
                throw new DatabaseException(ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<List<Prescription_Model>> GetAllPrescriptions(long patientId)
        {
            List<Prescription_Model> allPrescriptions = new List<Prescription_Model>();

           try
            {
                SqlCommand cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = System.Data.CommandType.Text,
                    CommandText = " SELECT TOP (1000) p.*, min(pa.AlertDateTime) NextAlert FROM MedScanRx.dbo.Prescription p " +
                                    " join PrescriptionAlert pa on pa.PrescriptionId = p.PrescriptionId " +
                                    " where p.PatientId = @patientId and IsActive = 1" +
                                    " group by p.PrescriptionId,Ndc,BrandName,GenericName,PatientId,Barcode,Color,Dosage,Identifier,Shape,DoctorNote, " +
                                    " Warning,OriginalNumberOfDoses,CurrentNumberOfDoses,OriginalNumberOfRefills,CurrentNumberOfRefills,IsActive," +
                                    " EnteredBy,EnteredDate,ModifiedBy,ModifiedDate" 
                };

                cmd.Parameters.AddWithValue("@patientId", patientId);

                await cn.OpenAsync().ConfigureAwait(false);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allPrescriptions.Add(DataRowToPrescriptionMapper.Map(reader));
                    }
                }

                return allPrescriptions;

            }
            catch(Exception ex)
            {
                throw new DatabaseException($"Something went wrong getting all prescriptions for patientId: {patientId}", ex);
            }
            finally
            {
                cn.Close();
            }
        }

    }
}
