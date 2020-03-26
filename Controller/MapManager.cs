using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TemPloy.Models;
using Xamarin.Forms.Maps;

namespace TemPloy.Controller
{
	class MapManager : Geocoder
	{
		//const string Url = "http://169.254.30.178:2345/api/";
		//const string Url = "http://192.168.43.102:2345/api/";
		//const string Url = "http://192.168.0.101:2345/api/";
		const string Url = "http://temployapi2017.azurewebsites.net/api/";

		public new async Task<IEnumerable<double>> GetPositionsForAddressAsync(string address)
		{
			using (var client = new HttpClient())
			{

				var request = string.Format(@"https://maps.googleapis.com/maps/api/geocode/json?address=" + address + "&key=AIzaSyB9_zi6sjCJFGJ6yNzi1lqUak5lHlFwb5Y");
				var json = await client.GetStringAsync(request);
				var result = JObject.Parse(json)["results"];
				List <double> list = new List<double>();

				foreach (JToken jt in result)
				{
					list.Add((double)jt["geometry"]["location"]["lat"]);
					list.Add((double)jt["geometry"]["location"]["lng"]);
				}
				return list.AsEnumerable<double>();
			}
		}

		public async Task<IEnumerable<Job>> GetAllJob()
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "job/");
				return JsonConvert.DeserializeObject<IEnumerable<Job>>(responseText);
			}
		}

		//get 1 ent
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

		//get ent by name and address
		public async Task<IEnumerable<Enterprise>> GetEntByNameAdd(string name,string address)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "enterprises?name=" + name + "&address=" + address);
				return JsonConvert.DeserializeObject<IEnumerable<Enterprise>>(responseText);
			}
		}

		//get all job based 1 ent
		public async Task<IEnumerable<Job>> GetJobs(string username)
		{
			using (var client = new HttpClient())
			{
				string responseText = await client.GetStringAsync(Url + "enterprise?username=" + username);
				return JsonConvert.DeserializeObject<IEnumerable<Job>>(responseText);
			}
		}
	}
}
