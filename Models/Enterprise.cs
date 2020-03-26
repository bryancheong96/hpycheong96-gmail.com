using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TemPloy.Models
{
	[DataContract]
	public class Enterprise 
	{
		[DataMember(Name = "entname")]
		public string CompanyName { get; set; }
		[DataMember(Name = "entid")]
		public string CompanyID { get; set; }
		[DataMember(Name = "entaddress")]
		public string CompanyAddress { get; set; }
		[DataMember(Name = "entcontact")]
		public string CompanyContact { get; set; }
		[DataMember(Name = "entusername")]
		public string Username { get; set; }
		[DataMember(Name = "entpassword")]
		public string Password { get; set; }
		[DataMember(Name = "entemail")]
		public string Email { get; set; }
		[DataMember(Name = "entstatus")]
		public string Status { get; set; }
		[DataMember(Name = "entremark")]
		public string Remark { get; set; }
		[DataMember(Name = "entphoto")]
		public string PhotoName { get; set; }

		public Enterprise(string CompanyName, string CompanyID, string CompanyAddress, string CompanyContact, string Username,
			string Password, string Email, string Status, string Remark, string PhotoName)
		{
			this.CompanyName = CompanyName;
			this.CompanyID = CompanyID;
			this.CompanyAddress = CompanyAddress;
			this.CompanyContact = CompanyContact;
			this.Username = Username;
			this.Password = Password;
			this.Email = Email;
			this.Status = Status;
			this.Remark = Remark;
			this.PhotoName = PhotoName;
		}

		public Enterprise() { }
	}
}
