using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TemPloy.Models
{
	[DataContract]
	public class Job
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }
		[DataMember(Name = "title")]
		public string Title { get; set; }
		[DataMember(Name = "jobdescription")]
		public string Description { get; set; }
		[DataMember(Name = "salary")]
		public decimal Salary { get; set; }
		[DataMember(Name = "salarytype")]
		public string SalaryType { get; set; }
		[DataMember(Name = "jobstatus")]
		public string Status { get; set; }
		[DataMember(Name = "entusername")]
		public string Entusername { get; set; }

		[DataMember(Name = "Submissions")]
		public IList<Submission> Submit { get; set; }

		public Job (int Id, string Title, string Description, decimal Salary, string SalaryType, string Status, string Entusername)
		{
			this.Id = Id;
			this.Title = Title;
			this.Description = Description;
			this.Salary = Salary;
			this.SalaryType = SalaryType;
			this.Status = Status;
			this.Entusername = Entusername;
		}
	}
}
