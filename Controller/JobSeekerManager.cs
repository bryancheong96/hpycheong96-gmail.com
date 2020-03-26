using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TemPloy.Models;
using TemPloy.Views;

namespace TemPloy.Controller
{
	class JobSeekerManager
	{
		//const string Url = "http://169.254.30.178:2345/api/";
		//const string Url = "http://192.168.43.102:2345/api/";
		//const string Url = "http://192.168.0.101:2345/api/";
		const string Url = "http://temployapi2017.azurewebsites.net/api/";

		public async Task<IEnumerable<JobSeeker>> GetAll()
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url+"jobseeker/");
				return JsonConvert.DeserializeObject<IEnumerable<JobSeeker>>(responseText);
			}
		}

		public async Task<IEnumerable<Job>> GetJobs()
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "job/");
				return JsonConvert.DeserializeObject<IEnumerable<Job>>(responseText);
			}
		}

		public async Task<JobSeeker> RegisterJobSeeker(string name, string username, string pass, string email, string status, string photo)
		{
			string remark = null;
			JobSeeker jobseeker = new JobSeeker(name, username, pass, email, status, remark, photo);
			using (var client = new HttpClient())
			{
				var convert = JsonConvert.SerializeObject(jobseeker);
				var content = new StringContent(convert, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(Url+"jobseekers/", content);
				response.EnsureSuccessStatusCode();
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<JobSeeker>(result);
			}
		}

		public async Task<JobSeeker> LoginJobSeeker(string username, string pass)
		{
			//JobSeeker login = new JobSeeker(username);
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url+"jobseekers?username="+username+"&password="+pass);
				if (responseText.IsSuccessStatusCode)
				{
					string responseBody = await responseText.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<JobSeeker>(responseBody);
				}
				else
				{
					JobSeeker empty = new JobSeeker();
					return empty;
				}
			}
		}

		public async Task<Submission> CreateSubmission(Submission submission)
		{
			using (var client = new HttpClient())
			{
				var convert = JsonConvert.SerializeObject(submission);
				var content = new StringContent(convert, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(Url + "jobseeker/", content);
				response.EnsureSuccessStatusCode();
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Submission>(result);
			}
		}

		public async Task<IEnumerable<Submission>> GetApplyJob(string username)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "jobseeker?username="+username);
				return JsonConvert.DeserializeObject<IEnumerable<Submission>>(responseText);
			}
		}

		//might affect, search for job
		/*
		public async Task<Job> GetEntById(int id)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "jobseeker?id=" + id);
				return JsonConvert.DeserializeObject<Job>(responseText);
			}
		}*/

		public async Task<Job> GetEntById(int id)
		{
			//JobSeeker login = new JobSeeker(username);
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url + "jobseeker?id=" + id);
				if (responseText.IsSuccessStatusCode)
				{
					string responseBody = await responseText.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<Job>(responseBody);
				}
				else
				{
					return null;
				}
			}
		}

		public async Task<EntFeedback> SaveEntFeedback(EntFeedback entfeedback)
		{
			using (var client = new HttpClient())
			{
				var convert = JsonConvert.SerializeObject(entfeedback);
				var content = new StringContent(convert, Encoding.UTF8, "application/json");
				var response = await client.PostAsync(Url + "entfeedback/", content);
				response.EnsureSuccessStatusCode();
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<EntFeedback>(result);
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

		public async Task<IEnumerable<EntFeedback>> CheckEntFeedback(int id)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "entfeedback?id=" + id);
				return JsonConvert.DeserializeObject<IEnumerable<EntFeedback>>(responseText);
			}
		}

		public async Task<Enterprise> GetEnterprise(string entusername)
		{
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url + "enterprise?entusername=" + entusername);
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

		public async Task<IEnumerable<JSFeedback>> GetAllJSFeedback(string username)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "jsfeedback?username=" + username);
				return JsonConvert.DeserializeObject<IEnumerable<JSFeedback>>(responseText);
			}
		}

		public async Task<JobSeeker> UpdateProfile(JobSeeker jobseeker)
		{
			using (var client = new HttpClient())
			{
				var response = await client.PutAsync(Url + "jobseeker/", new StringContent(JsonConvert.SerializeObject(jobseeker), Encoding.UTF8, "application/json"));
				var result = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<JobSeeker>(result);
			}
		}

		public async Task<Job> GetEntByJobid(int jobid)
		{
			//JobSeeker login = new JobSeeker(username);
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url + "enterprises?jobid=" + jobid);
				if (responseText.IsSuccessStatusCode)
				{
					string responseBody = await responseText.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<Job>(responseBody);
				}
				else
				{
					return null;
				}
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

		public async Task<JobSeeker> CheckUsername(string username)
		{
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url + "jobseekers?username=" + username);
				if (responseText.IsSuccessStatusCode)
				{
					string responseBody = await responseText.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<JobSeeker>(responseBody);
				}
				else
				{
					return null;
				}
			}
		}

		/*
		public async Task<IEnumerable<JobSeek>> GetAll()
		{
			HttpClient client = new HttpClient();
			// TODO: use GET to retrieve books
			//HttpClient client = await GetClient();
			string result = await client.GetStringAsync(Url);
			return JsonConvert.DeserializeObject<IEnumerable<JobSeek>>(result);
		}
		*/
	}
}
