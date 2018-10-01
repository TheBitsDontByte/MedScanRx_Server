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
	public enum Message
	{
		First,
		Second
	}

	public class CloudMessaging
	{

		private readonly AppPrescription_BLL _bll;

		public CloudMessaging(IConfiguration configuration)
		{
			_bll = new AppPrescription_BLL(configuration.GetConnectionString("MedScanRx_AWS"));
		}



		public async Task SendMessage(Message messageOrder)
		{

			List<PatientMessaging_Model> allPatientMessageInfo = messageOrder == Message.First
				? await _bll.GetInitialMessageInfo().ConfigureAwait(false)
				: await _bll.GetSecondMessageInfo().ConfigureAwait(false);

			foreach (var messageInfo in allPatientMessageInfo)
			{
				CreateAndSendMessage(messageInfo, messageOrder);
			}

		}

		public void CreateAndSendMessage(PatientMessaging_Model messageInfo, Message messageOrder)
		{
			string messageBody = "";

			string timeFromAlert = messageOrder == Message.First
				? $"{60 - DateTime.Now.Minute} minutes"
				: $"{DateTime.Now.Minute} minutes ago" ;

			messageBody = messageOrder == Message.First
				? messageInfo.NumberOfUpcomingAlerts > 1 ? $"You have {messageInfo.NumberOfUpcomingAlerts} medicines to take in about {timeFromAlert}" : $"You have a medicine to take in about {timeFromAlert}"
				: messageInfo.NumberOfUpcomingAlerts > 1 ? $"You have {messageInfo.NumberOfUpcomingAlerts} medicines to take about {timeFromAlert}" : $"You have a medicine to take about {timeFromAlert}";


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
					body = messageBody,
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
							}
					}
				}
			}
		}
	}
}


