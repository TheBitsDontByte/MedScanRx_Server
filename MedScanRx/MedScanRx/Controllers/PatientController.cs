using System;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.BLL;
using MedScanRx.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.Controllers
{
    [Authorize(Roles = "MedScanRx_Admin")]
    [Produces("application/json")]
    [Route("api/Patient/")]
    public class PatientController : Controller
    {
        public Patient_BLL _bll;
        public Auth_BLL _auth;

        public PatientController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("MedScanRx_AWS");
            _bll = new Patient_BLL(connectionString);
            _auth = new Auth_BLL(connectionString);
        }


        //NOT IN USE AND SET UP POORLY -- REDO BEFORE / IF USING
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
                    return NotFound(new { errors = $"No patient found with Patient ID {patientId}" });
                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }
        }

        [Route("SavePatient")]
        public async Task<IActionResult> SavePatient([FromBody] Patient_Model patient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage);
                    return BadRequest(new { errors = errors });
                }

                var patientAddedSuccess = await _bll.SavePatient(patient).ConfigureAwait(false);
                bool patientAccountAddedSuccess = await _auth.AddPatient(patient).ConfigureAwait(false);
                if (patientAddedSuccess && patientAccountAddedSuccess)
                    return Ok(patient);

                return BadRequest(new { errors = "Unable to save the patient information, please try again" } );

            } 
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }
        }

        [Route("UpdatePatient")]
        public async Task<IActionResult> UpdatePatient([FromBody]Patient_Model patient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(m => m.Errors).Select(m => m.ErrorMessage);
                    return BadRequest(new { errors = errors });
                }

                var success = await _bll.UpdatePatient(patient).ConfigureAwait(false);
                if (success)
                    return Ok(patient);

                return BadRequest(new { errors = "Unable to update the patient information, please try again" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }
        }


        //NOT IN USE CURRENTLY -- REDO LIKE ABOVE METHODS BEFORE USING
        [Route("DeletePatient/{patientId}")]
        public async Task<IActionResult> DeletePatient([FromRoute] long patientId)
        {
            try
            {
                var success = await _bll.DeletePatient(patientId).ConfigureAwait(false);
                if (success)
                    return Ok();

                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}