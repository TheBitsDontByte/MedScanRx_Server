using System.Net.Http;
using MedScanRx.BLL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedScanRx.Controllers
{

    [Produces("application/json")]
    [Route("api/Patient")]
    public class PatientController : Controller
    {
        public Patient_BLL _bll = new Patient_BLL();

        [Route("GetAllPatients")]
        public IActionResult GetAllPatients()
        {
            var allPatients = _bll.GetAllPatients();
        
            return Ok(allPatients);
        }

    }
}