using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TemPloy.Controller;
using TemPloy.Models;
using Xamarin.Forms;

namespace TemPloy.Views
{
	public class JobDetail : ContentPage
	{
		readonly Enterprise enterprise,entdetail;
		readonly JobSeeker jobseeker;
		readonly Job jobdetail;
		readonly Entry titleCell, titleCell1, salaryCell, salaryCell1;
		readonly Editor descriptionCell, descriptionCell1, proposalCell;
		readonly Label proposalCell2, length;
		readonly Picker salaryType, Status;
		readonly EnterpriseManager entmanager = new EnterpriseManager();
		readonly JobSeekerManager jsmanager = new JobSeekerManager();
		readonly IList<EntFeedback> entfeedbacks = new ObservableCollection<EntFeedback>();
		readonly IList<Submission> applies = new ObservableCollection<Submission>();
		readonly Submission submission;
		readonly string view;
		decimal a = 0;

		public JobDetail(Enterprise enterprise, JobSeeker jobseeker, Job jobdetail, Submission submission ,string view, Enterprise entdetail)
		{
			this.enterprise = enterprise;
			this.jobseeker = jobseeker;
			this.jobdetail = jobdetail;
			this.submission = submission;
			this.view = view;
			this.entdetail = entdetail;

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

			string salary = jobdetail.Salary.ToString();
			string empty = a.ToString();

			Button button = new Button()
			{
				BackgroundColor = Color.Green,
				TextColor = Color.White,
				Text = jobseeker != null ? "Apply" : "Save",
				BorderRadius = 10,
				VerticalOptions = LayoutOptions.End
			};
			button.Clicked += OnDismiss;

			Label Header = new Label
			{
				Text = "Job Detail",
				Font = Font.SystemFontOfSize(25),
				HorizontalOptions = LayoutOptions.Center
			};

			Label entnameLbl = new Label
			{
				Text = "Company Name"
			};
			Button entnameData = new Button
			{
				Text = entdetail.CompanyName,
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 45,
			};
			entnameData.FontAttributes = FontAttributes.Bold;
			entnameData.Clicked += ViewProfile;

			//Enterprise
			if (jobseeker == null)
			{
				Label titleLbl = new Label
				{
					Text = "Title"
				};
				Label titleCell = new Label
				{
					Text = jobdetail.Title
				};

				Label descriptionLbl = new Label
				{
					Text = "Description"
				};
				descriptionCell = new Editor
				{
					Text = jobdetail.Description,
					VerticalOptions = LayoutOptions.FillAndExpand,
				};

				Label salaryLbl = new Label
				{
					Text = "Salary"
				};
				salaryCell = new Entry
				{
					Text = salary
				};

				Label typeLbl = new Label
				{
					Text = "Salary Type"
				};
				List<string> Typeitem = new List<string> { "per day", "per hour" };
				salaryType = new Picker
				{
					Title = "Salary Type",
					ItemsSource = Typeitem,
					SelectedItem = jobdetail.SalaryType,
				};

				List<string> item = new List<string> { "Active", "End" };
				Label statusLbl = new Label
				{
					Text = "Status"
				};
				Status = new Picker
				{
					Title = "Status",
					ItemsSource = item,
					SelectedItem = jobdetail.Status,
				};

				if (jobdetail.Status.Equals("End"))
				{
					descriptionCell.IsEnabled = false;
					salaryCell.IsEnabled = false;
					salaryType.IsEnabled = false;
					Status.IsEnabled = false;
					button.IsVisible = false;
				}

				titleCell.IsEnabled = false;
				entnameData.IsEnabled = false; 

				Content = new ScrollView
				{
					Content = new StackLayout
					{
						VerticalOptions = LayoutOptions.FillAndExpand,
						Children = { Header, entnameLbl, entnameData, titleLbl, titleCell, descriptionLbl, descriptionCell, salaryLbl, salaryCell, typeLbl, salaryType, statusLbl, Status, button },
					}
				};
			} //Jobseeker
			else
			{
				Label titleLbl = new Label
				{
					Text = "Title"
				};
				Label titleCell1 = new Label
				{
					Text = jobdetail.Title
				};

				Label descriptionLbl = new Label
				{
					Text = "Description"
				};
				Label descriptionCell1 = new Label
				{
					Text = jobdetail.Description,
					VerticalOptions = LayoutOptions.FillAndExpand,
				};

				Label salaryLbl = new Label
				{
					Text = "Salary"
				};
				Label salaryCell1 = new Label
				{
					Text = salary + " " + jobdetail.SalaryType
				};

				Label proposalLbl = new Label
				{
					Text = "Proposal"
				};
				if (string.IsNullOrEmpty(view))
				{
					proposalCell = new Editor
					{
						Text =  null,
						HeightRequest = 150
					};
					length = new Label
					{
						Text = "Character length :"
					};
					proposalCell.TextChanged += (sender2, e2) => CheckChar(proposalCell.Text);
				}
				else
				{
					proposalCell2 = new Label
					{
						Text = submission.Proposal,
						VerticalOptions = LayoutOptions.FillAndExpand,
					};
				}

				List<string> item = new List<string> { "Active", "End" };
				Label statusLbl = new Label
				{
					Text = "Job Status"
				};
				Label status = new Label
				{
					Text = jobdetail.Status
				};

				if (string.IsNullOrEmpty(view))
				{
					Content = new ScrollView
					{
						Content = new StackLayout
						{
							VerticalOptions = LayoutOptions.FillAndExpand,
							Children = { Header, entnameLbl, entnameData, titleLbl, titleCell1, descriptionLbl, descriptionCell1, salaryLbl, salaryCell1, statusLbl, status, proposalLbl, proposalCell, length, button },
						}
					};
				}
				else
				{
					Content = new ScrollView
					{
						Content = new StackLayout
						{
							VerticalOptions = LayoutOptions.FillAndExpand,
							Children = { Header, entnameLbl, entnameData, titleLbl, titleCell1, descriptionLbl, descriptionCell1, salaryLbl, salaryCell1, statusLbl, status, proposalLbl, proposalCell2 },
						}
					};
				}
			}
		}

		private void CheckChar(string proposal)
		{
			length.Text = "Character length :" + proposal.Length.ToString();
			if(proposal.Length > 200)
			{
				proposalCell.BackgroundColor = Color.Red;
			}
			else
			{
				proposalCell.BackgroundColor = Color.Black;
			}
		}

		async void ViewProfile(object sender, EventArgs e)
		{
			double rate = 0;
			int divide = 0;
			string admin = null;

			var feedback = await entmanager.GetAllEntFeedback(jobdetail.Entusername);

			foreach (EntFeedback entfeedback in feedback)
			{
				if (entfeedbacks.All(b => b.Id != entfeedback.Id))
				{
					divide++;
					rate = rate + entfeedback.Rating;
				}
			}
			rate = rate / divide;
			await Navigation.PushModalAsync(new ProfileEnterprise(entdetail, jobseeker, rate, admin));
		}
		async void OnDismiss(object sender, EventArgs e)
		{
			Button button = (Button)sender;
			button.IsEnabled = false;
			this.IsBusy = true;
			try
			{
				int Id = 0;
				string status = "Pending";
				int jobid = jobdetail.Id;

				if (jobseeker != null)
				{
					string proposal = proposalCell.Text.TrimEnd();

					if (string.IsNullOrEmpty(proposal))
					{
						await DisplayAlert("Alert Message", "Please fill up the proposal.", "Cancel");
					}
					else if(proposalCell.Text.Length > 200)
					{
						await DisplayAlert("Alert Message", "Porposal statement should not be more than 200 characters.", "Cancel");
					}
					else
					{
						int count = 0;
						var checkjs = await jsmanager.GetApplyJob(jobseeker.Username);

						foreach(Submission applied in checkjs)
						{
							if(applies.All(b => b.Id != applied.Id))
							{
								if(applied.Jobid == jobdetail.Id)
								{
									applies.Add(applied);
									count++;
								}
							}	
						}

						if(count == 0)
						{
							//apply
							Submission submission = new Submission(Id, status, proposal, jobdetail.Title, jobid, jobseeker.Name, jobseeker.Username);
							var submit = await jsmanager.CreateSubmission(submission);
							if (submit != null)
							{
								await DisplayAlert("Apply Successful", "Application submitted.", "Cancel");
								await Navigation.PopModalAsync();
							}
							else
							{
								await DisplayAlert("Apply Failed", "Application fail to submit. Something wents wrong.", "Cancel");
								await Navigation.PopModalAsync();
							}
						}
						else
						{
							await DisplayAlert("Apply Failed", "You have already apply for this job.", "Cancel");
							await Navigation.PopModalAsync();
						}

					}
				}
				else
				{
					string description = descriptionCell.Text.TrimEnd();
					string convert = salaryCell.Text.TrimEnd();
					string salarytype = salaryType.Items[salaryType.SelectedIndex];
					string jobstatus = Status.Items[Status.SelectedIndex];

					if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(convert))
					{
						await DisplayAlert("Alert Message", "Please fill up the job description and salary.", "Cancel");
					}
					else
					{
						try
						{
							decimal salary1 = decimal.Parse(convert);
							decimal salary = Math.Round(salary1, 2);

							if(salary <= 0 || salary > 210000)
							{
								await DisplayAlert("Alert Message", "Salary must be greater than 0 and lesser than 210,000.", "Cancel");
							}
							else
							{
								if (jobstatus.Equals("End"))
								{
									if(await this.DisplayAlert("End Job?", "You can't change the job details anymore once you have end the job's status. Are you sure, you still want to proceed?", "Yes", "Cancel") == true)
									{
										//update
										Job job = new Job(jobdetail.Id, jobdetail.Title, description, salary, salarytype, jobstatus, jobdetail.Entusername);
										var update = await entmanager.UpdateJob(job);
										if (update != null)
										{
											await DisplayAlert("Update Successful", "Application updated.", "Cancel");
											await Navigation.PopModalAsync();
										}
										else
										{
											await DisplayAlert("Update Failed", "Application fail to update.", "Cancel");
											await Navigation.PopModalAsync();
										}
									}
								}
								else
								{
									//update
									Job job = new Job(jobdetail.Id, jobdetail.Title, description, salary, salarytype, jobstatus, jobdetail.Entusername);
									var update = await entmanager.UpdateJob(job);
									if (update != null)
									{
										await DisplayAlert("Update Successful", "Application updated.", "Cancel");
										await Navigation.PopModalAsync();
									}
									else
									{
										await DisplayAlert("Update Failed", "Application fail to update.", "Cancel");
										await Navigation.PopModalAsync();
									}
								}
							}
						}
						catch (Exception ex)
						{
							await DisplayAlert("Alert Message", "Something wents wrong.", "Cancel");
						}
					}
				}
			}
			finally
			{
				this.IsBusy = false;
				button.IsEnabled = true;
			}
		}

		async void BackPage(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}
	}
}