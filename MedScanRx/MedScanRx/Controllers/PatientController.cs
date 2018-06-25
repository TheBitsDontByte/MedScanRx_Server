using System;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.BLL;
using MedScanRx.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedScanRx.Controllers
{

    [Produces("application/json")]
    [Route("api/Patient/")]
    public class PatientController : Controller
    {
        public Patient_BLL _bll = new Patient_BLL();


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

                var success = await _bll.SavePatient(patient).ConfigureAwait(false);
                if (success)
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