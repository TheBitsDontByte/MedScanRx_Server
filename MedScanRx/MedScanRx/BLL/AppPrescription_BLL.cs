﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.DAL;
using MedScanRx.Models;

namespace MedScanRx.BLL
{
    public class AppPrescription_BLL
    {
        AppPrescription_DAL _dal = new AppPrescription_DAL();

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
    }
}