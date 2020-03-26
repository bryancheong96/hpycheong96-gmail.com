using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TemPloy.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateJob : ContentPage
	{
		readonly Enterprise enterprise;
		readonly EnterpriseManager entmanager = new EnterpriseManager();

		public CreateJob(Enterprise enterprise)
		{
			this.enterprise = enterprise;
			InitializeComponent();

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
					this.Padding = new Thickness(10, 0, 10, 5);
					break;
			}

		}

		async void CreateJobStatement(object sender, EventArgs e)
		{
			int Id = 0;
			string Title = title.Text;
			string Description = description.Text;
			string Salary = salary.Text;
			string SalaryType = salarytype.Items[salarytype.SelectedIndex];
			string Status = "Active";

			Regex whitespaceregex = new Regex(@"\s{1,}");
			Regex titleregex = new Regex("[A-Za-z0-9]{1,50}");
			Regex salaryregex = new Regex("[A-Za-z]{1,}");

			if (!(string.IsNullOrEmpty(Title)) && !(string.IsNullOrEmpty(Description)) && (!(string.IsNullOrEmpty(Salary))) && !(string.IsNullOrEmpty(SalaryType)))
			{
				if (Title.Length > 100)
				{
					await DisplayAlert("Alert Message", "Title only accept 100 alphanumeric characters.", "Cancel");
				}
				else if (whitespaceregex.IsMatch(Salary.ToString()))
				{
					await DisplayAlert("Alert Message", "No space is allowed for salary and salary type", "Cancel");
				}
				else if (salaryregex.IsMatch(Salary.ToString()))
				{
					await DisplayAlert("Alert Message", "Salary only accept numeric characters.", "Cancel");
				}
				else
				{
					try
					{
						decimal doubleSalary = decimal.Parse(Salary);
						decimal Salary2 =  Math.Round(doubleSalary, 2);

						var register = await entmanager.CreateJob(Id, Title, Description, Salary2, SalaryType, Status, enterprise.Username);
						if (register != null)
						{
							await DisplayAlert("Job Creation", "Job Created", "Cancel");
							await Navigation.PopModalAsync();
						}
						else
						{
							await DisplayAlert("Job Creation", "Job fail to create.", "Cancel");
						}
					}catch (Exception ex)
					{
						await DisplayAlert("Error Message", "Salary only accept up 6-digits.", "Cancel");
					}
				}
			}
			else
			{
				await DisplayAlert("Alert Message", "Please fill up all the details.", "Cancel");
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}