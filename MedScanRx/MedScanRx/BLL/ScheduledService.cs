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
			bool initialMessageHasBeenSent = false;
			bool secondMessageHasBeenSent = false;

			while (!cancellationToken.IsCancellationRequested)
			{
				if (DateTime.Now.Minute > 35 && !deactivateHasRunThisHour)
				{
					await _deactivatePastAlerts.Deactivate(cancellationToken);
					deactivateHasRunThisHour = true;
				}

				if (DateTime.Now.Minute > 45 && !initialMessageHasBeenSent)
				{
					await _cloudMessaging.SendMessage(Message.First);
					initialMessageHasBeenSent = true;
					secondMessageHasBeenSent = false;
				}

				if (DateTime.Now.Minute > 10 && DateTime.Now.Minute < 15 && !secondMessageHasBeenSent)
				{
					await _cloudMessaging.SendMessage(Message.Second);
					secondMessageHasBeenSent = true;
				}

				if (DateTime.Now.Minute < 10 && deactivateHasRunThisHour && initialMessageHasBeenSent)
				{
					deactivateHasRunThisHour = !deactivateHasRunThisHour;
					initialMessageHasBeenSent = !initialMessageHasBeenSent;
				}
				await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);

			}
		}
	}
}
