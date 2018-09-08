using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MedScanRx.Models;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.DAL
{
	public class Auth_DAL
	{
		private static SqlConnection cn;

		public Auth_DAL(string connectionString)
		{
			cn = new SqlConnection(connectionString);
		}

		public bool AuthenticateAdmin(LoginRequest loginRequest)
		{
			try
			{
				return login("AdminAccount", loginRequest);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				cn.Close();
			}
		}

		public int GetPatientId(LoginRequest loginRequest)
		{
			try
			{
				SqlCommand cmd = new SqlCommand
				{
					Connection = cn,
					CommandType = System.Data.CommandType.Text,
					CommandText = "SELECT PatientId FROM PatientAccount WHERE UserName = @UserName AND PW = @PW "
				};

				cmd.Parameters.AddWithValue("@UserName", loginRequest.UserName);
				cmd.Parameters.AddWithValue("@PW", loginRequest.Password);

				cn.Open();
				return (int)cmd.ExecuteScalar();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				cn.Close();
			}
		}

		public async Task UpdateFcmToken(string fcmToken)
		{
			try
			{
				SqlCommand cmd = new SqlCommand
				{
					Connection = cn,
					CommandType = System.Data.CommandType.Text,
					CommandText = "UPDATE PatientAccount SET FcmToken = @FcmToken"
				};
				cmd.Parameters.AddWithValue("@FcmToken", fcmToken);


				cn.Open();
				cmd.ExecuteNonQueryAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				cn.Close();
			}
		}

		public bool AuthenticatePatient(LoginRequest loginRequest)
		{
			try
			{
				return login("PatientAccount", loginRequest);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				cn.Close();
			}
		}

		public async Task<bool> AddPatientAsync(Patient_Model patient)
		{
			try
			{
				SqlCommand cmd = new SqlCommand
				{
					CommandType = System.Data.CommandType.Text,
					Connection = cn,
					CommandText = "INSERT INTO dbo.PatientAccount (PatientId, UserName, PW, Salt) VALUES (@PatientId, @UserName, @PW, @Salt) "
				};

				cmd.Parameters.AddWithValue("@PatientId", patient.PatientId);
				cmd.Parameters.AddWithValue("@UserName", patient.Email);
				cmd.Parameters.AddWithValue("@PW", patient.DateOfBirth.ToString("MMddyyyy"));
				cmd.Parameters.AddWithValue("@Salt", "Wat ?");

				cn.Open();
				return await cmd.ExecuteNonQueryAsync().ConfigureAwait(false) == 1;

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				cn.Close();
			}
		}

		private bool login(string database, LoginRequest loginRequest)
		{
			SqlCommand cmd = new SqlCommand
			{
				CommandType = System.Data.CommandType.Text,
				Connection = cn,
				CommandText = $"SELECT COUNT(*) FROM dbo.{database} WHERE UserName = @UserName and PW = @Password"
			};

			cmd.Parameters.AddWithValue("@UserName", loginRequest.UserName);
			cmd.Parameters.AddWithValue("@Password", loginRequest.Password);

			cn.Open();
			return (int)cmd.ExecuteScalar() == 1;
		}
	}
}
