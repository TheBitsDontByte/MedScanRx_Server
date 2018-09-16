using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MedScanRx.BLL;
using MedScanRx.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MedScanRx.ScheduledServices
{
	public class CloudMessaging
	{

		private readonly Prescription_BLL _bll;

		public CloudMessaging(IConfiguration configuration)
		{
			_bll = new Prescription_BLL(configuration);
		}



		public async Task SendInitialMessage()
		{

			List<PatientMessaging_Model> allPatientMessageInfo = await _bll.GetInitialMessageInfo().ConfigureAwait(false);

			foreach (var messageInfo in allPatientMessageInfo)
			{
				SendMessage(messageInfo);
			}

		}

		public void SendMessage(PatientMessaging_Model messageInfo)
		{

			WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
			tRequest.Method = "post";
			//serverKey - Key from Firebase cloud messaging server  
			tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAZJgbXy0:APA91bFqr2ryc6uBrtfSid645h1lFadS0iStbpMFg7oAUCat1AWQQ8ZcTRu8wrzRQzOzgLjsg6sYF-T2HZ7xPXxaJ9xnORXax26HDkxEazunlOE68FAAWA2qgwlIFsTwNoig-LStS2gc"));
			//Sender Id - From firebase project setting  
			tRequest.Headers.Add(string.Format("Sender: id={0}", "432048660269"));
			tRequest.ContentType = "application/json";
			var payload = new
			{
				to = messageInfo.FcmToken,
				priority = "high",
				content_available = true,
				notification = new
				{
					body =  messageInfo.NumberOfUpcomingAlerts > 1 ? $"You have {messageInfo.NumberOfUpcomingAlerts} medicines to take in about 15 minutes" : $"You have a medicine to take in about 15 minutes",
					title = "MedScanRx Upcoming Alerts",
					badge = 1,
				},

			};

			string postbody = JsonConvert.SerializeObject(payload).ToString();
			Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
			tRequest.ContentLength = byteArray.Length;
			using (Stream dataStream = tRequest.GetRequestStream())
			{
				dataStream.Write(byteArray, 0, byteArray.Length);
				using (WebResponse tResponse = tRequest.GetResponse())
				{
					using (Stream dataStreamResponse = tResponse.GetResponseStream())
					{
						if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
							{
								String sResponseFromServer = tReader.ReadToEnd();
								//result.Response = sResponseFromServer;
							}
					}
				}
			}
			//end

		}
	}
}


