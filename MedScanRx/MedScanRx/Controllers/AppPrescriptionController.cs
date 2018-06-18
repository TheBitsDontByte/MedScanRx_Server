using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedScanRx.Controllers
{
    [Produces("application/json")]
    [Route("AppApi/Prescription/")]
    public class AppPrescriptionController : Controller
    {
        AppPrescription_BLL _bll = new AppPrescription_BLL();


        [HttpGet]
        [Route("UpcomingAlerts/{patientId}")]
        public async Task<IActionResult> UpcomingAlerts(long patientId)
        {
            try
            {
                var allUpcomingPrescriptions = await _bll.GetUpcomingAlerts(patientId).ConfigureAwait(false);
                return Ok(allUpcomingPrescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = "Server error trying to get upcoming alerts " });
            }

        }

        [HttpGet]
        [Route("AllPrescriptions/{patientId}")]
        public async Task<IActionResult> AllPrescriptions(long patientId)
        {
            try
            {
                var allPrescriptions = await _bll.GetAllPrescriptions(patientId).ConfigureAwait(false);
                return Ok(allPrescriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = "Server error trying to get all prescription information" });
            }
        }

    }
}