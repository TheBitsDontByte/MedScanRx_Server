using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MedScanRx.DAL;
using MedScanRx.Models;

namespace MedScanRx.BLL
{
    public class Prescription_BLL
    {
        Prescription_DAL _dal = new Prescription_DAL();

		public async Task<string> SearchOpenFda(OpenFdaSearch_Model model)
        {
            HttpClient client = new HttpClient();
            string url = openfdaUrlBuilder(model);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            return  await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

		public async Task<bool> SavePrescription(Prescription_Model model)
        {
            model.PrescriptionId = await _dal.SavePrescription(model).ConfigureAwait(false);
            return await _dal.SavePrescriptionAlerts(model).ConfigureAwait(false);
            
        }

        public async Task<List<Prescription_Model>> GetAllPrescriptions(long patientId)
        {
            return await _dal.GetAllPrescriptions(patientId).ConfigureAwait(false);
        }

        public async Task<Prescription_Model> GetPrescription(int prescriptionId)
        {
            var model = await _dal.GetPrescription(prescriptionId).ConfigureAwait(false);
            model.ScheduledAlerts = await _dal.GetPrescriptionAlerts(prescriptionId).ConfigureAwait(false);
            return model;
        }

        public async Task<bool> UpdatePrescription(Prescription_Model model)
        {
            var prescriptionSuccess = await _dal.UpdatePrescription(model).ConfigureAwait(false);
            var alertSuccess = await _dal.UpdatePrescriptionAlerts(model).ConfigureAwait(false);

            return (alertSuccess && prescriptionSuccess);
            
        }
        
        public async Task<bool> DeletePrescriptionAndAlerts(long patientId, int prescriptionId)
        {
            return await _dal.DeletePrescriptionAndAlerts(patientId, prescriptionId).ConfigureAwait(false);
        }

        private string openfdaUrlBuilder(OpenFdaSearch_Model search)
        {
            string baseUrl = @"https://api.fda.gov/drug/label.json?search=";
            string builtUrl = baseUrl;

            if (!string.IsNullOrEmpty(search.BrandName))
                builtUrl += $"openfda.brand_name:{search.BrandName}+";
            if (!string.IsNullOrEmpty(search.GenericName))
                builtUrl += $"openfda.generic_name:{search.GenericName}+";
            if (!string.IsNullOrEmpty(search.Ndc))
                builtUrl += $"openfda.package_ndc:\"{search.Ndc}\"+";

            builtUrl += "&limit=15";

            return builtUrl;
        }
    }
}
