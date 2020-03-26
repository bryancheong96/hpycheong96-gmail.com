using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TemPloy.Models;

namespace TemPloy.Controller
{
	class EnterpriseManager
	{
		//const string Url = "http://169.254.30.178:2345/api/";
		//const string Url = "http://192.168.43.102:2345/api/";
		//const string Url = "http://192.168.0.101:2345/api/";
		const string Url = "http://temployapi2017.azurewebsites.net/api/";

		public async Task<Enterprise> RegisterEnterprise(string name, string id, string address, string contact, string username,
			string password, string email, string status, string photoname)
		{
			string remark = null;
			Enterprise enterprise = new Enterprise(name, id, address, contact, username, password, email, status, remark, photoname);
			using (var client = new HttpClient())
			{
				var convert = JsonConvert.SerializeObject(enterprise);
				var content = new StringContent(convert, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(Url + "enterprise/", content);
				response.EnsureSuccessStatusCode();
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Enterprise>(result);
			}
		}

		public async Task<Enterprise> LoginEnterprise(string username, string pass)
		{
			//JobSeeker login = new JobSeeker(username);
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url + "enterprise?username=" + username + "&password=" + pass);
				if (responseText.IsSuccessStatusCode)
				{
					string responseBody = await responseText.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<Enterprise>(responseBody);
				}
				else
				{
					Enterprise empty = new Enterprise();
					return empty;
				}
			}
		}

		public async Task<Job> CreateJob(int Id,string Title, string Description, decimal Salary, string SalaryType, string Status, string Entusername)
		{
			Job job = new Job(Id, Title, Description, Salary, SalaryType, Status, Entusername);
			using (var client = new HttpClient())
			{
				var convert = JsonConvert.SerializeObject(job);
				var content = new StringContent(convert, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(Url + "job/", content);
				response.EnsureSuccessStatusCode();
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Job>(result);
			}
		}

		//Enterprise Homepage - My Job
		public async Task<IEnumerable<Job>> GetJobs(string username)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "enterprise?username="+username);
				return JsonConvert.DeserializeObject<IEnumerable<Job>>(responseText);
			}
		}

		public async Task<Job> UpdateJob(Job job)
		{
			using (var client = new HttpClient())
			{
				var response = await client.PutAsync(Url + "enterprise/",new StringContent(JsonConvert.SerializeObject(job),Encoding.UTF8, "application/json"));
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Job>(result);
			}
		}

		//might Affect
		public async Task<IEnumerable<Job>> GetSubmission(int id)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "job?id=" + id);
				return JsonConvert.DeserializeObject<IEnumerable<Job>>(responseText);
			}
		}

		public async Task<Submission> UpdateSubmission(Submission submission)
		{
			using (var client = new HttpClient())
			{
				var response = await client.PutAsync(Url + "job/", new StringContent(JsonConvert.SerializeObject(submission), Encoding.UTF8, "application/json"));
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Submission>(result);
			}
		}

		public async Task<JobSeeker> GetJobSeekerByUsername(string seekerusername)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "enterprise?seekerusername=" + seekerusername);
				return JsonConvert.DeserializeObject<JobSeeker>(responseText);
			}
		}

		public async Task<JSFeedback> SaveFeedback(JSFeedback jsfeedback)
		{
			using (var client = new HttpClient())
			{
				var convert = JsonConvert.SerializeObject(jsfeedback);
				var content = new StringContent(convert, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(Url + "jsfeedback/", content);
				response.EnsureSuccessStatusCode();
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<JSFeedback>(result);
			}
		}

		public async Task<IEnumerable<JSFeedback>> CheckJSFeedback(int id)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "jsfeedback?id=" + id);
				return JsonConvert.DeserializeObject<IEnumerable<JSFeedback>>(responseText);
			}
		}

		public async Task<IEnumerable<EntFeedback>> GetAllEntFeedback(string username)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "entfeedback?username=" + username);
				return JsonConvert.DeserializeObject<IEnumerable<EntFeedback>>(responseText);
			}
		}

		public async Task<Enterprise> UpdateEnterprise(Enterprise enterprise)
		{
			using (var client = new HttpClient())
			{
				var response = await client.PutAsync(Url + "enterprises/", new StringContent(JsonConvert.SerializeObject(enterprise), Encoding.UTF8, "application/json"));
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Enterprise>(result);
			}
		}

		public async Task<IEnumerable<Enterprise>> CheckId(string companyid)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "enterprise?companyid=" + companyid);
				return JsonConvert.DeserializeObject<IEnumerable<Enterprise>>(responseText);
			}
		}

		public async Task<string> SendPhoto(MediaFile mediafile)
		{
			var content = new MultipartFormDataContent();
			content.Add(new StreamContent(mediafile.GetStream()), "\"file\"", $"\"{mediafile.Path}\"");
			using (var httpclient = new HttpClient())
			{
				//var uploadservice = "http://169.254.30.178:2345/api/imageupload";
				var uploadservice = Url+"/imageupload";
				var httpResponseMessage = await httpclient.PostAsync(uploadservice, content);
				string text = await httpResponseMessage.Content.ReadAsStringAsync();
				return text;
			}
		}

		public async Task<Enterprise> CheckUsername(string username)
		{
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url + "enterprises?username=" + username);
				if (responseText.IsSuccessStatusCode)
				{
					string responseBody = await responseText.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<Enterprise>(responseBody);
				}
				else
				{
					return null;
				}
			}
		}
	}
}
