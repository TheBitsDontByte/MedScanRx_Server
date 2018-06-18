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
                cmd.Parameters.AddWithValue("@DoctorNote", model.DoctorNotes ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Warning", model.Warnings ?? (object)DBNull.Value);
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
                return (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

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


                string cmdString = "INSERT INTO PrescriptionAlert (PrescriptionId, AlertDateTime, IsActive) VALUES ";
                cmd.Parameters.AddWithValue("@PrescriptionId", model.PrescriptionId);
                cmd.Parameters.AddWithValue("@IsActive", 1);

                for (int i = 0; i < model.ScheduledAlerts.Count; i++)
                {
                    cmdString += $"(@PrescriptionId, @ScheduledAlert{i}, @IsActive),";
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
                    CommandText = " SELECT  p.*, min(pa.AlertDateTime) as NextAlert FROM Prescription p " +
                                    " join PrescriptionAlert pa on pa.PrescriptionId = p.PrescriptionId " +
                                    " where p.PatientId = @patientId and p.IsActive = 1 and pa.IsActive = 1" +
                                    " group by p.PrescriptionId,Ndc,BrandName,GenericName,PatientId,Barcode,Color,Dosage,Identifier,Shape,DoctorNote, " +
                                    " Warning,OriginalNumberOfDoses,CurrentNumberOfDoses,OriginalNumberOfRefills,CurrentNumberOfRefills,p.IsActive," +
                                    " EnteredBy,EnteredDate,ModifiedBy,ModifiedDate"
                };

                cmd.Parameters.AddWithValue("@patientId", patientId);

                await cn.OpenAsync().ConfigureAwait(false);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allPrescriptions.Add(DataRowToAllPrescriptionsMapper.Map(reader));
                    }
                }

                return allPrescriptions;

            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Something went wrong getting all prescriptions for patientId: {patientId}", ex);
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

        public async Task<bool> UpdatePrescription(Prescription_Model model)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    Connection = cn,
                    CommandText = "UPDATE Prescription SET Ndc = @Ndc, BrandName = @BrandName, GenericName = @GenericName, PatientId = @PatientId, Barcode = @Barcode, Color = @Color, " +
                                    "Dosage = @Dosage, Identifier = @Identifier, Shape = @Shape, DoctorNote = @DoctorNote, Warning = @Warning, CurrentNumberOfDoses = @CurrentNumberOfDoses, " +
                                    "CurrentNumberOfRefills = @CurrentNumberOfRefills, IsActive = @IsActive, ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate " +
                                    " where PrescriptionId = @PrescriptionId"
                };

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
                cmd.Parameters.AddWithValue("@CurrentNumberOfDoses", model.CurrentNumberOfDoses);
                cmd.Parameters.AddWithValue("@CurrentNumberOfRefills", model.CurrentNumberOfRefills);
                cmd.Parameters.AddWithValue("@IsActive", 1); //TODO handle this
                cmd.Parameters.AddWithValue("@ModifiedBy", "Where to get this user");
                cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@PrescriptionId", model.PrescriptionId);


                await cn.OpenAsync().ConfigureAwait(false);
                return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Something went wrong updating the prescription", ex);
            }
            finally
            {
                cn.Close();
            }

        }

        public async Task<bool> UpdatePrescriptionAlerts(Prescription_Model model)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    Connection = cn,
                    CommandText =
                    "DELETE FROM PrescriptionAlert where PrescriptionId = @PrescriptionId AND AlertDateTime > GETDATE()"
                };

                cmd.Parameters.AddWithValue("@PrescriptionId", model.PrescriptionId);
                await cn.OpenAsync().ConfigureAwait(false);

                var rowsDeleted = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                if (rowsDeleted != model.ScheduledAlerts.Count)
                    throw new InvalidOperationException("Incorrect number of rows deleted");

                cmd.CommandText = "INSERT INTO PrescriptionAlert (PrescriptionId, AlertDateTime, IsActive) VALUES ";
                for (int i = 0; i < model.ScheduledAlerts.Count; i++)
                {
                    cmd.CommandText += $" (@PrescriptionId, @AlertDateTime{i}, @IsActive), ";
                    cmd.Parameters.AddWithValue($"@AlertDateTime{i}", model.ScheduledAlerts.ElementAt(i));
                }
                cmd.Parameters.AddWithValue("@IsActive", 1);

                cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.LastIndexOf(","), 1);

                var rowsAdded = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                if (rowsAdded != model.ScheduledAlerts.Count)
                    throw new InvalidOperationException("Incorrect number of rows added/updated");

                return true;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Something went wrong updating the prescription alerts", ex);
            }
            finally
            {
                cn.Close();
            }
        }

        public async Task<bool> DeletePrescriptionAndAlerts(long patientId, int prescriptionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandType = System.Data.CommandType.Text,
                    Connection = cn,
                    CommandText =
                        "UPDATE PrescriptionAlert SET IsActive = @IsActive  WHERE PrescriptionId = @PrescriptionId AND AlertDateTime > GETDATE() AND IsActive = 1; " +
                        "UPDATE Prescription SET IsActive = @IsActive, ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate WHERE PatientId = @PatientId AND PrescriptionId = @PrescriptionId AND IsActive = 1; "
                };

                cmd.Parameters.AddWithValue("@PatientId", patientId);
                cmd.Parameters.AddWithValue("@PrescriptionId", prescriptionId);
                cmd.Parameters.AddWithValue("@IsActive", 0);
                cmd.Parameters.AddWithValue("@ModifiedBy", "User here");
                cmd.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);

                await cn.OpenAsync().ConfigureAwait(false);
                var rowsDelted = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                return true;

            } catch (Exception ex)
            {
                throw new DatabaseException("Something went wrong deleting the prescription", ex);
            } finally
            {
                cn.Close();
            }
        }
    }
}
