using System.Collections.Generic;
using System.Threading.Tasks;
using MedScanRx.DAL;
using MedScanRx.Models;

namespace MedScanRx.BLL
{
    public class Patient_BLL
    {
        private static Patient_DAL _dal;

        public Patient_BLL(string connectionString)
        {
            _dal = new Patient_DAL(connectionString);
        }

        public async Task<List<Patient_Model>> GetAllPatients()
        {
            return await _dal.GetAllPatients().ConfigureAwait(false);
        }

        public async Task<Patient_Model> GetPatient(long patientId)
        {
            return await _dal.GetPatient(patientId).ConfigureAwait(false);
        }

        public async Task<bool> SavePatient(Patient_Model patient)
        {
            return await _dal.SavePatient(patient).ConfigureAwait(false);
        }

        public async Task<bool> UpdatePatient(Patient_Model patient)
        {
            return await _dal.UpatePatient(patient).ConfigureAwait(false);
        }

        public async Task<bool> DeletePatient(long patientId)
        {
            return await _dal.DeletePatient(patientId).ConfigureAwait(false);
        }
    }
}
