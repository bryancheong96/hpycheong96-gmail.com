using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TemPloy.Models
{
	[DataContract]
	public class Admin
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
