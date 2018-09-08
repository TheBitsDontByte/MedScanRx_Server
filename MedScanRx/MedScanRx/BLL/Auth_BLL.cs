using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.DAL;
using MedScanRx.Models;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.BLL
{
	public class Auth_BLL
	{
		private Auth_DAL _dal;

		public Auth_BLL(string connectionString)
		{
			_dal = new Auth_DAL(connectionString);
		}

		public bool AuthenticateAdmin(LoginRequest loginRequest)
		{
			return _dal.AuthenticateAdmin(loginRequest);
		}

		public int GetPatientId(LoginRequest loginRequest)
		{
			return _dal.GetPatientId(loginRequest);
		}

		public bool AuthenticatePatient(LoginRequest loginRequest)
		{
			return _dal.AuthenticatePatient(loginRequest);
		}

		public async Task<bool> AddPatient(Patient_Model patient)
		{
			return await _dal.AddPatientAsync(patient).ConfigureAwait(false);
		}

		public async Task UpdateFcmToken(string fcmToken)
		{
			_dal.UpdateFcmToken(fcmToken);
		}
	}
}
