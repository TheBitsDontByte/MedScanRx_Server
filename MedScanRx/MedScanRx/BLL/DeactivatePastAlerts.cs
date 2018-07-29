using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MedScanRx.BLL
{
    public class DeactivatePastAlerts
    {
        private readonly Prescription_BLL _bll;

        public DeactivatePastAlerts(IConfiguration configuration)
        {
            _bll = new Prescription_BLL(configuration);
        }

        public async Task Deactivate(CancellationToken cancellationToken)
        {
            await _bll.DeactivatePastAlerts();
        }
    }
}
