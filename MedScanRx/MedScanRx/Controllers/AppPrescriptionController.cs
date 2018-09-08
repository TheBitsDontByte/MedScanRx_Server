using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MedScanRx.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.Controllers
{
    [Authorize(Roles = "MedScanRx_Patient")]
    [Produces("application/json")]
    [Route("Api/App/Prescription/")]
    public class AppPrescriptionController : Controller
    {
        private AppPrescription_BLL _bll;

        public AppPrescriptionController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("MedScanRx_AWS");
            _bll = new AppPrescription_BLL(connectionString);
        }

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
            var x = this.User.Claims.First(c => c.Type == "patientId");
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

        [HttpGet]
        [Route("AllPrescriptionsWithAlerts/{prescriptionId}")]
        public async Task<IActionResult> PrescriptionWithAlerts(int prescriptionId)
        {
            try
            {
                var allPrescriptionsWithAlerts = await _bll.GetPrescriptionWithAlerts(prescriptionId).ConfigureAwait(false);
                return Ok(allPrescriptionsWithAlerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorMessage = "Server error trying to get all prescriptions and alert information" });
            }
        }

    }
}