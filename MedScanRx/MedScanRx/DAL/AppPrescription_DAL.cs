using System;
using System.Collections.Generic;
using System.Configuration;
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
		private static SqlConnection cn;

		public AppPrescription_DAL(string connectionString)
		{
			cn = new SqlConnection(connectionString);
		}

		public async Task<List<Prescription_Model>> GetUpcomingAlerts(long patientId)
		{
			List<Prescription_Model> allUpcomingPrescriptions = new List<Prescription_Model>();

			try
			{
				SqlCommand cmd = new SqlCommand
				{
					CommandType = System.Data.CommandType.StoredProcedure,
					Connection = cn,
					CommandText = "sp_get_upcoming_alerts_for_patient"
				};

				cmd.Parameters.AddWithValue("@PatientId", patientId);

				await cn.OpenAsync().ConfigureAwait(false);
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						allUpcomingPrescriptions.Add(DataRowToUpcomingPrescriptionMapper.Map(reader));
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

		public async Task<bool> TakeMedicine(int prescriptionAlertId)
		{
			try
			{
				SqlCommand cmd = new SqlCommand
				{
					CommandType = System.Data.CommandType.Text,
					Connection = cn,
					CommandText = " UPDATE PrescriptionAlert set TakenDateTime = GETUTCDATE(), IsActive = 0 " +
												" where PrescriptionAlertId = @PrescriptionAlertId "
				};

				cmd.Parameters.AddWithValue("@PrescriptionAlertId", prescriptionAlertId);

				await cn.OpenAsync().ConfigureAwait(false);
				return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;

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
					CommandText = " SELECT p.*, min(pa.AlertDateTime) as NextAlert FROM Prescription p " +
														" JOIN PrescriptionAlert pa on pa.PrescriptionId = p.PrescriptionId " +
														" WHERE p.PatientId = @PatientId AND pa.IsActive = 1 AND p.IsActive = 1 " +
														" group by p.PrescriptionId,Ndc,PrescriptionName,PatientId,Color,Dosage,Identifier,Shape,Rxcui,ImageUrl,DoctorNote, " +
														" Warning,OriginalNumberOfDoses,CurrentNumberOfDoses,OriginalNumberOfRefills,CurrentNumberOfRefills,p.IsActive, " +
														" EnteredBy,EnteredDate,ModifiedBy,ModifiedDate"
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
					CommandText = " SELECT p.*, min(pa.AlertDateTime) as NextAlert FROM Prescription p " +
														" JOIN PrescriptionAlert pa on pa.PrescriptionId = p.PrescriptionId " +
														" WHERE p.PrescriptionId = @PrescriptionId AND pa.IsActive = 1 AND p.IsActive = 1 " +
														" group by p.PrescriptionId,Ndc,PrescriptionName,PatientId,Color,Dosage,Identifier,Shape,Rxcui,ImageUrl,DoctorNote, " +
														" Warning,OriginalNumberOfDoses,CurrentNumberOfDoses,OriginalNumberOfRefills,CurrentNumberOfRefills,p.IsActive, " +
														" EnteredBy,EnteredDate,ModifiedBy,ModifiedDate"

				};
				cmd.Parameters.AddWithValue("@PrescriptionId", prescriptionId);

				await cn.OpenAsync().ConfigureAwait(false);
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						model = DataRowToAllPrescriptionsMapper.Map(reader);
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
					CommandText = $"SELECT AlertDateTime, IsActive, TakenDateTime FROM PrescriptionAlert WHERE PrescriptionId = @prescriptionId"
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

		public async Task<List<PrescriptionAlert_Model>> GetPrescriptionActiveAlerts(int prescriptionId)
		{
			List<PrescriptionAlert_Model> alerts = new List<PrescriptionAlert_Model>();

			try
			{
				SqlCommand cmd = new SqlCommand
				{
					Connection = cn,
					CommandType = System.Data.CommandType.Text,
					CommandText = $"SELECT AlertDateTime, IsActive, TakenDateTime FROM PrescriptionAlert WHERE PrescriptionId = @prescriptionId and IsActive = 1"
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

		public async Task<List<PatientMessaging_Model>> GetInitialMessageInfo()
		{
			try
			{
				List<PatientMessaging_Model> allPatientMessageInfo = new List<PatientMessaging_Model>();

				SqlCommand cmd = new SqlCommand
				{
					CommandType = System.Data.CommandType.Text,
					Connection = cn,
					CommandText = "SELECT patient.FcmToken, COUNT(pa.PrescriptionAlertId) as NumberOfUpcomingAlerts, pa.AlertDateTime " +
												" FROM PrescriptionAlert pa " +
												" JOIN Prescription p ON pa.PrescriptionId = p.PrescriptionId " +
												" JOIN PatientAccount patient on patient.PatientId = p.PatientId " +
												" WHERE (pa.AlertDateTime BETWEEN GETUTCDATE() AND DATEADD(MINUTE, 20, GETUTCDATE())) AND pa.IsActive = 1 " +
												"GROUP BY patient.FcmToken, pa.AlertDateTime	"
				};

				await cn.OpenAsync().ConfigureAwait(false);
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						allPatientMessageInfo.Add(DataRowToPatientMessagingMapper.Map(reader));
					}
				}

				return allPatientMessageInfo;

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

		public async Task<List<PatientMessaging_Model>> GetSecondMessageInfo()
		{
			try
			{
				List<PatientMessaging_Model> allPatientMessageInfo = new List<PatientMessaging_Model>();

				SqlCommand cmd = new SqlCommand
				{
					CommandType = System.Data.CommandType.Text,
					Connection = cn,
					CommandText = "SELECT patient.FcmToken, COUNT(pa.PrescriptionAlertId) as NumberOfUpcomingAlerts, pa.AlertDateTime " +
												" FROM PrescriptionAlert pa " +
												" JOIN Prescription p ON pa.PrescriptionId = p.PrescriptionId " +
												" JOIN PatientAccount patient on patient.PatientId = p.PatientId " +
												" WHERE (pa.AlertDateTime BETWEEN DATEADD(MINUTE, -15, GETUTCDATE()) AND GETUTCDATE()) AND pa.IsActive = 1 " +
												"GROUP BY patient.FcmToken, pa.AlertDateTime	"
				};

				await cn.OpenAsync().ConfigureAwait(false);
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						allPatientMessageInfo.Add(DataRowToPatientMessagingMapper.Map(reader));
					}
				}

				return allPatientMessageInfo;

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


	}
}
