using System;
using System.Data.SqlClient;
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
