using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedScanRx.ScheduledServices;

namespace MedScanRx.BLL
{
	public class ScheduledService : MyHostedService
	{
		private readonly DeactivatePastAlerts _deactivatePastAlerts;
		private readonly CloudMessaging _cloudMessaging;

		public ScheduledService(DeactivatePastAlerts deactivatePastAlerts, CloudMessaging cloudMessaging)
		{
			_deactivatePastAlerts = deactivatePastAlerts;
			_cloudMessaging = cloudMessaging;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			bool deactivateHasRunThisHour = false;
			bool initialMessageHasBeenSentThisHour = false;

			while (!cancellationToken.IsCancellationRequested)
			{
				//Only run deactivate if it's 35 or 36 past. Numerous runs should produce the same results

				if (DateTime.Now.Minute > 35 && !deactivateHasRunThisHour)
				{
					await _deactivatePastAlerts.Deactivate(cancellationToken);
					deactivateHasRunThisHour = true;
				}
				if (DateTime.Now.Minute > 45 && !initialMessageHasBeenSentThisHour)
				{
					await _cloudMessaging.SendInitialMessage();
					initialMessageHasBeenSentThisHour = true;
				}
				if (DateTime.Now.Minute < 10 && deactivateHasRunThisHour && initialMessageHasBeenSentThisHour)
				{
					deactivateHasRunThisHour = !deactivateHasRunThisHour;
					initialMessageHasBeenSentThisHour = !initialMessageHasBeenSentThisHour;
				}
				await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

			}
		}
	}
}
