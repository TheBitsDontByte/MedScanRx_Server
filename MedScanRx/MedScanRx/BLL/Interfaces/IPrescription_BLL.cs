using System.Collections.Generic;
using System.Threading.Tasks;
using MedScanRx.Models;

namespace MedScanRx.BLL.Interfaces
{
    public interface IPrescription_BLL
    {
        Task DeactivatePastAlerts();
        Task<bool> DeletePrescriptionAndAlerts(long patientId, int prescriptionId);
        Task<List<Prescription_Model>> GetAllPrescriptions(long patientId);
        Task<Prescription_Model> GetPrescription(int prescriptionId);
        Task<bool> SavePrescription(Prescription_Model model);
        Task<string> SearchOpenFda(OpenFdaSearch_Model model);
        Task<string> SearchOpenFda(string rxcui);
        Task<string> SearchRxcui(OpenFdaSearch_Model model);
        Task<bool> UpdatePrescription(Prescription_Model model);
    }
}