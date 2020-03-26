using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TemPloy.Models
{
	[DataContract]
	public class Submission
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }
		[DataMember(Name = "appstatus")]
		public string Status { get; set; }
		[DataMember(Name = "proposal")]
		public string Proposal { get; set; }
		[DataMember(Name = "title")]
		public string Title { get; set; }
		[DataMember(Name = "jobid")]
		public int Jobid { get; set; }
		[DataMember(Name = "seekername")]
		public string Name { get; set; }
		[DataMember(Name = "seekerusername")]
		public string Username { get; set; }
		/*
		public IList<JobSeeker> JobSeeker { get; set; }*/

		public Submission(int Id, string Status, string Proposal, string Title, int Jobid, string Name, string Username)
		{
			this.Id = Id;
			this.Status = Status;
			this.Proposal = Proposal;
			this.Title = Title;
			this.Jobid = Jobid;
			this.Name = Name;
			this.Username = Username;
		}
	}
}
