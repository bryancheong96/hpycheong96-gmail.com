using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Threading;

namespace TemPloy.Views
{
	public class MapPage : ContentPage
	{
		Geocoder geoCoder;
		private Entry searchLbl;
		private double longitude, latitude;
		//private double lat = 4.210484, lng = 101.975766;
		private double lat = 0, lng = 0;
		private Map map;
		readonly public Label geocodedOutputLabel;
		readonly MapManager mapmanager = new MapManager();
		readonly JobSeeker jobseeker;
		readonly IList<double> latList = new ObservableCollection<double>();
		readonly IList<double> lngList = new ObservableCollection<double>();
		readonly IList<Job> jobs = new ObservableCollection<Job>();

        public MapPage()
		{
			//this.jobseeker = jobseeker;
			SearchGPS();
			OnSearch();

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					var a = new ToolbarItem
					{
						Text = "back",

					};
					a.Clicked += BackPage;

					ToolbarItems.Add(a);
					break;
				default:
					break;
			}
		}

		async void SearchGPS()
		{
			try
			{
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;

				var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(1000), null);

				//await DisplayAlert("Update Complete", position.ToString(), "Cancel");

				if (position == null)
				{
					await DisplayAlert("Location", "no value", "Cancel");
					lat = position.Latitude;
					lng = position.Longitude;
				}
				else
				{
					lat = position.Latitude;
					lng = position.Longitude;
				}
			}
			catch (Exception e)
			{
				await DisplayAlert("Location", "Location fail to detect.", "Cancel");
			}
		}

		async void OnSearch()
		{
            
			this.IsBusy = true;

			try
			{
				var alljobs = await mapmanager.GetAllJob();
				foreach (Job job in alljobs)
				{
					if (jobs.All(b => b.Id != job.Id))
					{
						if (jobs.All(b => b.Entusername != job.Entusername))
						{
							if (job.Status.Equals("Active"))
							{
								jobs.Add(job);
							}
						}
					}
				}
			}
			finally
			{
				this.IsBusy = false;
			}


			map = new Map(
				MapSpan.FromCenterAndRadius(
				new Xamarin.Forms.Maps.Position(lat, lng), Distance.FromMiles(5)))
			{
				IsShowingUser = true,
				//HeightRequest = 300,
				//WidthRequest = 250,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			searchLbl = new Entry()
			{
				Placeholder = "Enter City/Town"
			};

			Button searchbtn = new Button()
			{
				//BackgroundColor = existingBook != null ? Color.Gray : Color.Green,
				TextColor = Color.White,
				Text = "Search",
				BorderRadius = 0, // corner of the button
				HeightRequest = 40,
			};
			searchbtn.Clicked += ChangeLocation;

			var stack = new StackLayout { Spacing = 0 };
			stack.Children.Add(searchLbl);
			stack.Children.Add(searchbtn);
			stack.Children.Add(map);
			//stack.HorizontalOptions = LayoutOptions.FillAndExpand;
			Content = new ScrollView
			{
				Content = stack
			};


			Pin[] locate = new Pin[jobs.Count];

			for (int i=0; i < jobs.Count; i++)
			{
				var getent = await mapmanager.GetEnterprise(jobs[i].Entusername);

				geoCoder = new Geocoder();
				int count = 0;

				var approximateLocations = await mapmanager.GetPositionsForAddressAsync(getent.CompanyAddress);
				foreach (double s in approximateLocations)
				{
					if (count == 0)
					{
						lat = s;
						count++;
					}
					else if (count == 1)
					{
						lng = s;
						count = 0;
					}
				}

				locate[i] = new Pin
				{
					Type = PinType.Place,
					Position = new Xamarin.Forms.Maps.Position(lat, lng),
					Label = getent.CompanyName,
					Address = getent.CompanyAddress,
				};
				map.Pins.Add(locate[i]);
				locate[i].Clicked += ViewJob;

			}



			/*
			geoCoder = new Geocoder();
			int count = 0, next=0;

			var approximateLocations = await mapmanager.GetPositionsForAddressAsync(address);
			foreach (double s in approximateLocations)
			{
				if(count == 0)
				{
					lat = s;
					await DisplayAlert("Login Failed", lat.ToString(), "Re-Login");
					count++;
				}
				else if (count == 1)
				{
					lng = s;
					await DisplayAlert("Login Failed", lng.ToString(), "Re-Login");
					count = 0;
					next++;
				}
			}

			Pin[] locate = new Pin[next];

			for(int i=0; i<next; i++)
			{
				locate[i] = new Pin
				{
					Type = PinType.Place,
					Position = new Position(lat,lng),
					
				};
			}

			var map = new Map(
				MapSpan.FromCenterAndRadius(
						new Position(lat, lng), Distance.FromMiles(0.3)))
			{
				IsShowingUser = true,
				//HeightRequest = 50,
				//WidthRequest = 250,
				VerticalOptions = LayoutOptions.FillAndExpand
			};
			var stack = new StackLayout { Spacing = 0 };
			stack.Children.Add(map);
			Content = stack;

			var position = new Position(lat, lng); // Latitude, Longitude
			var pin = new Pin
			{
				Type = PinType.Place,
				Position = position,
				Label = "custom pin",
				Address = "custom detail info"
			};
			map.Pins.Add(pin);*/
		}

		async void ViewJob(object sender, EventArgs e)
		{
			Pin a = sender as Pin;

			await Navigation.PushModalAsync(new NavigationPage(new HomeJobSeeker(jobseeker, a.Label, a.Address)));
		}

		async void ChangeLocation(object sender, EventArgs e)
		{
			string text = searchLbl.Text;
			int count = 0;

			var approximateLocations = await mapmanager.GetPositionsForAddressAsync(text);
			foreach (double s in approximateLocations)
			{
				if (count == 0)
				{
					latitude = s;
					count++;
				}
				else if (count == 1)
				{
					longitude = s;
					count = 0;
				}
			}

			map.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(latitude, longitude), Distance.FromKilometers(6)));

		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}