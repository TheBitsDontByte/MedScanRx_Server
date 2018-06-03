using System.Collections.Generic;
using System.Threading.Tasks;
using MedScanRx.DAL;
using MedScanRx.Models;

namespace MedScanRx.BLL
{
    public class Patient_BLL
    {
        private static Patient_DAL _dal = new Patient_DAL();

        public async Task<List<Patient_Model>> GetAllPatients()
        {
            return await _dal.GetAllPatients().ConfigureAwait(false);
        }

        public async Task<Patient_Model> GetPatient(long patientId)
        {
            return await _dal.GetPatient(patientId).ConfigureAwait(false);
        }
    }
}
