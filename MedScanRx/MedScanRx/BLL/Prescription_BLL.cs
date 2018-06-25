using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
            //string url = c3piUrlBuilder(model);
            string url = openfdaUrlBuilder(model);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            return  await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<string> SearchRxcui(OpenFdaSearch_Model model)
        {
            HttpClient client = new HttpClient();
            string url = c3piUrlBuilder(model);
            
            HttpResponseMessage response = await client.GetAsync(url);
           

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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

        private string c3piUrlBuilder(OpenFdaSearch_Model search)
        {
            string baseUrl = "https://rximage.nlm.nih.gov/api/rximage/1/rxnav?";
            string builtUrl = baseUrl;

            if (!string.IsNullOrEmpty(search.Name))
                builtUrl += $"name={search.Name}&";
            if (!string.IsNullOrEmpty(search.Ndc))
                builtUrl += $"ndc={search.Ndc}";

            return builtUrl;
            
        }


        //Deprecated - but here as reference for what was before - no longer using openfda
        private string openfdaUrlBuilder(OpenFdaSearch_Model search)
        {
            string baseUrl = @"https://api.fda.gov/drug/label.json?search=";
            string builtUrl = baseUrl;

            if (!string.IsNullOrEmpty(search.Name))
                builtUrl += $"openfda.brand_name:\"{search.Name}\"+";
            //if (!string.IsNullOrEmpty(search.GenericName))
            //    builtUrl += $"openfda.generic_name:{search.GenericName}+";
            if (!string.IsNullOrEmpty(search.Ndc))
                builtUrl += $"openfda.package_ndc:\"{search.Ndc}\"+";

            builtUrl += "&limit=30";

            return builtUrl;
        }
    }
}
