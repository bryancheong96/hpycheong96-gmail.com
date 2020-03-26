using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TemPloy.Models;

namespace TemPloy.Controller
{
	class AdminManager
	{
		//const string Url = "http://169.254.30.178:2345/api/";
		//const string Url = "http://192.168.43.102:2345/api/";
		//const string Url = "http://192.168.0.101:2345/api/";
		const string Url = "http://temployapi2017.azurewebsites.net/api/";

		public async Task<Admin> LoginAdmin(string username, string pass)
		{
			using (var client = new HttpClient())
			{
				HttpResponseMessage responseText = await client.GetAsync(Url + "admin?username=" + username + "&password=" + pass);
				if (responseText.IsSuccessStatusCode)
				{
					string responseBody = await responseText.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<Admin>(responseBody);
				}
				else
				{
					return null;
				}
			}
		}

		public async Task<IEnumerable<Enterprise>> GetAllEnt()
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "admin/");
				return JsonConvert.DeserializeObject<IEnumerable<Enterprise>>(responseText);
			}
		}
		public async Task<IEnumerable<JobSeeker>> GetAllJobSeeker()
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "admin?user");
				return JsonConvert.DeserializeObject<IEnumerable<JobSeeker>>(responseText);
			}
		}
	}
}
