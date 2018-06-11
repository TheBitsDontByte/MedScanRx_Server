﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using MedScanRx.BLL;
using MedScanRx.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedScanRx.Controllers
{
    [Produces("application/json")]
    [Route("api/Prescription/")]
    public class PrescriptionController : Controller
    {
        Prescription_BLL _bll = new Prescription_BLL();

        [Route("Search")]
        public async Task<IActionResult> SearchOpenFda([FromBody] OpenFdaSearch_Model model)
        {
            try
            {


                var result = await _bll.SearchOpenFda(model).ConfigureAwait(false);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Route("Save")]
        public async Task<IActionResult> SavePrescription([FromBody] Prescription_Model model)
        {
            var result = await _bll.SavePrescription(model).ConfigureAwait(false);

            if (result)
                return Ok();

            return BadRequest();

        }

      

    }
}