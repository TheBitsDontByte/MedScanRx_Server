using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.DAL;
using MedScanRx.Models;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.BLL
{
	public class AppPrescription_BLL
	{
		private AppPrescription_DAL _dal;

		public AppPrescription_BLL(string connectionString)
		{
			_dal = new AppPrescription_DAL(connectionString);
		}

		public async Task<List<Prescription_Model>> GetUpcomingAlerts(long patientId)
		{
			return await _dal.GetUpcomingAlerts(patientId).ConfigureAwait(false);
		}

		public async Task<List<Prescription_Model>> GetAllPrescriptions(long patientId)
		{
			return await _dal.GetAllPrescriptions(patientId).ConfigureAwait(false);
		}

		public async Task<Prescription_Model> GetPrescriptionWithAlerts(int prescriptionId)
		{
			var model = await _dal.GetPrescription(prescriptionId).ConfigureAwait(false);
			model.ScheduledAlerts = await _dal.GetPrescriptionAlerts(prescriptionId).ConfigureAwait(false);
			return model;
		}

		public async Task<Prescription_Model> GetPrescriptionWithActiveAlerts(int prescriptionId)
		{
			var model = await _dal.GetPrescription(prescriptionId).ConfigureAwait(false);
			model.ScheduledAlerts = await _dal.GetPrescriptionActiveAlerts(prescriptionId).ConfigureAwait(false);
			return model;
		}

		public async Task<bool> TakeMedicine(int prescriptionAlertId)
		{
			return await _dal.TakeMedicine(prescriptionAlertId);
		}

		public async Task<List<PatientMessaging_Model>> GetInitialMessageInfo()
		{
			return await _dal.GetInitialMessageInfo().ConfigureAwait(false);
		}

		public async Task<List<PatientMessaging_Model>> GetSecondMessageInfo()
		{
			return await _dal.GetSecondMessageInfo().ConfigureAwait(false);
		}
	}
}
