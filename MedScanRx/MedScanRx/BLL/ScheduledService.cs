using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MedScanRx.BLL
{
    public class ScheduledService : MyHostedService
    {
        private readonly DeactivatePastAlerts _deactivatePastAlerts;

        public ScheduledService(DeactivatePastAlerts deactivatePastAlerts)
        {
            _deactivatePastAlerts = deactivatePastAlerts;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _deactivatePastAlerts.Deactivate(cancellationToken);
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                
            }
        }
    }
}
