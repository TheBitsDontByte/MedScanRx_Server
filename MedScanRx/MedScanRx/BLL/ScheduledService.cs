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
				//Only run deactivate if it's 35 past. Numerous runs should produce the same results
				if (DateTime.Now.Minute > 35)
				{
					await _deactivatePastAlerts.Deactivate(cancellationToken);
					await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
				} else
				{
					await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
				}

			}
		}
	}
}
