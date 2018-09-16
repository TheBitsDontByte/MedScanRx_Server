using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MedScanRx.Exceptions;
using MedScanRx.Models;

namespace MedScanRx.DAL.Mapper
{
	public class DataRowToPatientMessagingMapper
	{
		public static PatientMessaging_Model Map(SqlDataReader reader)
		{
			try
			{
				var messagingInfo = new PatientMessaging_Model
				{
					AlertDateTime = DateTime.Parse(reader["AlertDateTime"].ToString()),
					FcmToken = reader["FcmToken"].ToString(),
					NumberOfUpcomingAlerts = (int)reader["NumberOfUpcomingAlerts"]
				};



				return messagingInfo;

			}
			catch (Exception ex)
			{
				throw new MappingException("Mapping a data row to a patient message info failed", ex);
			}
		}

	}
}

