using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedScanRx.Models
{
	public class PatientMessaging_Model
	{
		public string FcmToken { get; set; }
		public int NumberOfUpcomingAlerts { get; set; }
		public DateTime AlertDateTime { get; set; }
	}
}
