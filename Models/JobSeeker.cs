using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TemPloy.Models
{
	[DataContract]
	public class JobSeeker
	{
		[DataMember(Name ="seekerusername")]
		public string Username { get; set; }
		[DataMember(Name = "seekername")]
		public string Name { get; set; }
		[DataMember(Name = "seekerpassword")]
		public string Password { get; set; }
		[DataMember(Name = "seekeremail")]
		public string Email { get; set;}
		[DataMember(Name = "seekerstatus")]
		public string Status { get; set; }
		[DataMember(Name = "seekerremark")]
		public string Remark { get; set; }
		[DataMember(Name = "seekerphoto")]
		public string PhotoName { get; set; }

		public JobSeeker(string Name, string Username, string b, string Email, string Status, string Remark, string PhotoName)
		{
			this.Name = Name;
			this.Username = Username;
			this.Password = b;
			this.Email = Email;
			this.Status = Status;
			this.Remark = Remark;
			this.PhotoName = PhotoName;
		}

		public JobSeeker() {}
	}
}
