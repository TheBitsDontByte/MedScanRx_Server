using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using MedScanRx.BLL;
using MedScanRx.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.Controllers
{
    [Produces("application/json")]
    [Route("api/Prescription/")]
    public class PrescriptionController : Controller
    {
        Prescription_BLL _bll;

        public PrescriptionController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("MedScanRx_AWS");
            _bll = new Prescription_BLL(connectionString);
        }

        [Route("SearchOpenfda")]
        public async Task<IActionResult> SearchOpenFda([FromBody] OpenFdaSearch_Model model)
        {
            try
            {
                var result = await _bll.SearchOpenFda(model).ConfigureAwait(false);
                if (result == null)
                    return NotFound(new { errors = $"No medicines found with those search terms" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }

        }

        [Route("SearchOpenfda/{rxcui}")]
        public async Task<IActionResult> SearchOpenFda(string rxcui)
        {
            try
            {
                var result = await _bll.SearchOpenFda(rxcui).ConfigureAwait(false);
                if (result == null)
                    return NotFound(new { errors = $"No medicines found with that rxcui" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }

        }

        [Route("SearchRxcui")]
        public async Task<IActionResult> SearchRxcui([FromBody] OpenFdaSearch_Model model)
        {
            try
            {
                var result = await _bll.SearchRxcui(model).ConfigureAwait(false);
                if (result == null)
                    return NotFound(new { errors = $"No medicines found with those search terms" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }

        }

        [Route("Save")]
        public async Task<IActionResult> SavePrescription([FromBody] Prescription_Model model)
        {
            try
            {
                var result = await _bll.SavePrescription(model).ConfigureAwait(false);

                if (result)
                    return Ok();

                return BadRequest(new { errors = "Unable to save the prescription, please try again" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }

        }

        [Route("Prescriptions/{patientId}")]
        public async Task<IActionResult> GetAllPrescriptions(long patientId)
        {
            try
            {
                var allPrescriptions = await _bll.GetAllPrescriptions(patientId).ConfigureAwait(false);

                return Ok(allPrescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = ex.Message });
            }
        }

        [Route("Prescription/{prescriptionId}")]
        public async Task<IActionResult> GetPrescription(int prescriptionId)
        {
            try
            {
                var prescription = await _bll.GetPrescription(prescriptionId).ConfigureAwait(false);
                return Ok(prescription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = "Something went wrong getting the prescription details" });
            }
        }

        [Route("UpdatePrescription")]
        public async Task<IActionResult> UpdatePrescription([FromBody] Prescription_Model model)
        {
            try
            {
                var result = await _bll.UpdatePrescription(model).ConfigureAwait(false);

                if (result)
                    return Ok();

                return BadRequest(new { errors = "Unable to update the prescription, please try again" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errors = "Something went wrong updating the prescription details" });
            }
        }

        [HttpDelete]
        [Route("DeletePrescription")]
        public async Task<IActionResult> DeletePrescription([FromQuery] int prescriptionId, [FromQuery] long patientId)
        {
            try
            {
                var success = await _bll.DeletePrescriptionAndAlerts(patientId, prescriptionId).ConfigureAwait(false);
                if (success)
                    return Ok();

                return BadRequest(new { errors = "Unable to delete the prescription, please try again" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong deleting the prescription");
            }
        }

    }
}