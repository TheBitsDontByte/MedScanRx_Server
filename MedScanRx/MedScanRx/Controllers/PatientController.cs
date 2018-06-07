using System;
using System.Net.Http;
using System.Threading.Tasks;
using MedScanRx.BLL;
using MedScanRx.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedScanRx.Controllers
{

    [Produces("application/json")]
    [Route("api/Patient/")]
    public class PatientController : Controller
    {
        public Patient_BLL _bll = new Patient_BLL();

        [Route("Patients")]
        public IActionResult GetAllPatients()
        {
            var allPatients = _bll.GetAllPatients();

            return Ok(allPatients);
        }

        [Route("{patientId}")]
        public async Task<IActionResult> GetPatient([FromRoute]long patientId)
        {
            try
            {
                var patient = await _bll.GetPatient(patientId).ConfigureAwait(false);
                if (patient == null)
                    return NoContent();
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("SavePatient")]
        public async Task<IActionResult> SavePatient([FromBody] Patient_Model patient)
        {
            try
            {
                var x = this;
                var success = await _bll.SavePatient(patient).ConfigureAwait(false);
                if (success)
                    return Ok(patient);

                return BadRequest();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("UpdatePatient")]
        public async Task<IActionResult> UpdatePatient([FromBody]Patient_Model patient)
        {
            try
            {
                var success = await _bll.UpdatePatient(patient).ConfigureAwait(false);
                if (success)
                    return Ok(patient);

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}