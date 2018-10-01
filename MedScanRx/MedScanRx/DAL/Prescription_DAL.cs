using System;
using System.Collections;
using MedScanRx.Models;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.Exceptions;
using System.Collections.Generic;
using MedScanRx.DAL.Mapper;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.DAL
{
	public class Prescription_DAL
	{
		private static SqlConnection cn;

		public Prescription_DAL(IConfiguration configuration)
		{
			cn = new SqlConnection(configuration.GetConnectionString("MedScanRx_AWS"));
		}

		public async Task<int> SavePrescription(Prescription_Model model)
		{
			try
			{
				SqlCommand cmd = new SqlCommand
				{
					Connection = cn,
					CommandType = System.Data.CommandType.Text,
					CommandText = "INSERT INTO [dbo].[Prescription] " +
																"([Ndc], PrescriptionName, [PatientId],[Color],[Dosage],[Identifier]," +
																"[Shape], [Rxcui], [ImageUrl], [DoctorNote],[Warning],[OriginalNumberOfDoses],[CurrentNumberOfDoses]," +
																"[OriginalNumberOfRefills],[CurrentNumberOfRefills],[IsActive],[EnteredBy]," +
																"[EnteredDate],[ModifiedBy],[ModifiedDate])" +
																"OUTPUT inserted.PrescriptionId " +
																"VALUES(@Ndc, @PrescriptionName, @PatientId, @Color, @Dosage, @Identifier, " +
																"@Shape, @Rxcui, @ImageUrl, @DoctorNote, @Warning, @OriginalNumberOfDoses, @CurrentNumberOfDoses, @OriginalNumberOfRefills, " +
																"@CurrentNumberOfRefills, @IsActive, @EnteredBy, @EnteredDate, @ModifiedBy, @ModifiedDate)"
				};

				DateTime now = DateTime.Now;

				cmd.Parameters.AddWithValue("@Ndc", model.Ndc);
				cmd.Parameters.AddWithValue("@PrescriptionName", model.PrescriptionName);
				cmd.Parameters.AddWithValue("@PatientId", model.PatientId);
				cmd.Parameters.AddWithValue("@Color", model.Color);
				cmd.Parameters.AddWithValue("@Dosage", model.Dosage);
				cmd.Parameters.AddWithValue("@Identifier", model.Identifiers);
				cmd.Parameters.AddWithValue("@Shape", model.Shape);
				cmd.Parameters.AddWithValue("@Rxcui", model.Rxcui ?? (object)DBNull.Value);
				cmd.Parameters.AddWithValue("@ImageUrl", model.ImageUrl ?? (object)DBNull.Value);
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
				throw new DatabaseException("Something went wrong saving the prescription", ex);
			}
			finally
			{
				cn.Close();
			}
		}

		public async Task DeactivatePastAlerts()
		{
			try
			{
				SqlCommand cmd = new SqlCommand
				{
					CommandType = System.Data.CommandType.StoredProcedure,
					Connection = cn,
					CommandText = "sp_deactivate_past_alerts"

				};
				//SqlCommand cmd = new SqlCommand
				//            {
				//                CommandType = System.Data.CommandType.Text,
				//                Connection = cn,
				//                CommandText = "update prescriptionalert set isactive = 1"
				//            };

				cn.Open();
				cmd.ExecuteNonQuery();

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
					cmd.Parameters.AddWithValue($"@ScheduledAlert{i}", model.ScheduledAlerts[i].AlertDateTime);
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
														" group by p.PrescriptionId, Ndc, PrescriptionName, " +
														" PatientId, Color, Dosage, Identifier, Shape, Rxcui, ImageUrl, DoctorNote, " +
														" Warning, OriginalNumberOfDoses, CurrentNumberOfDoses, OriginalNumberOfRefills, CurrentNumberOfRefills, p.IsActive," +
														" EnteredBy, EnteredDate, ModifiedBy, ModifiedDate"
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

		public async Task<List<PrescriptionAlert_Model>> GetPrescriptionAlerts(int prescriptionId)
		{
			List<PrescriptionAlert_Model> alerts = new List<PrescriptionAlert_Model>();

			try
			{
				SqlCommand cmd = new SqlCommand
				{
					Connection = cn,
					CommandType = System.Data.CommandType.Text,
					CommandText = $"SELECT AlertDateTime, TakenDateTime, IsActive FROM PrescriptionAlert WHERE PrescriptionId = @prescriptionId"
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
					CommandText = "UPDATE Prescription SET PrescriptionName = @PrescriptionName,  Color = @Color," +
														" Dosage = @Dosage, Identifier = @Identifier, Shape = @Shape, DoctorNote = @DoctorNote," +
														" Warning = @Warning, CurrentNumberOfDoses = @CurrentNumberOfDoses, CurrentNumberOfRefills = @CurrentNumberOfRefills," +
														" ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate " +
														" where PrescriptionId = @PrescriptionId"
				};

				cmd.Parameters.AddWithValue("@PrescriptionName", model.PrescriptionName);
				cmd.Parameters.AddWithValue("@Color", model.Color);
				cmd.Parameters.AddWithValue("@Dosage", model.Dosage);
				cmd.Parameters.AddWithValue("@Identifier", model.Identifiers);
				cmd.Parameters.AddWithValue("@Shape", model.Shape);
				cmd.Parameters.AddWithValue("@DoctorNote", model.DoctorNotes);
				cmd.Parameters.AddWithValue("@Warning", model.Warnings);
				cmd.Parameters.AddWithValue("@CurrentNumberOfDoses", model.CurrentNumberOfDoses);
				cmd.Parameters.AddWithValue("@CurrentNumberOfRefills", model.CurrentNumberOfRefills);
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
						"DELETE FROM PrescriptionAlert where PrescriptionId = @PrescriptionId AND IsActive = 1"
				};

				cmd.Parameters.AddWithValue("@PrescriptionId", model.PrescriptionId);
				await cn.OpenAsync().ConfigureAwait(false);
				//Just delete all upcoming ones, nothing to really check against ?
				await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);


				cmd.CommandText = "INSERT INTO PrescriptionAlert (PrescriptionId, AlertDateTime, IsActive) VALUES ";
				for (int i = 0; i < model.ScheduledAlerts.Count; i++)
				{
					cmd.CommandText += $" (@PrescriptionId, @AlertDateTime{i}, @IsActive), ";
					cmd.Parameters.AddWithValue($"@AlertDateTime{i}", model.ScheduledAlerts[i].AlertDateTime);
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
								"UPDATE PrescriptionAlert SET IsActive = @IsActive  WHERE PrescriptionId = @PrescriptionId AND IsActive = 1; " +
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

			}
			catch (Exception ex)
			{
				throw new DatabaseException("Something went wrong deleting the prescription", ex);
			}
			finally
			{
				cn.Close();
			}
		}
	}
}
