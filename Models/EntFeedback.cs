using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TemPloy.Models
{
	[DataContract]
	class EntFeedback
	{
		[DataMember(Name = "id")]
		public int Id { get; set; }
		[DataMember(Name = "entname")]
		public string EntName { get; set; }
		[DataMember(Name = "entusername")]
		public string EntUsername { get; set; }
		[DataMember(Name = "seekername")]
		public string SeekerName { get; set; }
		[DataMember(Name = "seekerusername")]
		public string SeekerUsername { get; set; }
		[DataMember(Name = "submissionid")]
		public int Submissionid { get; set; }
		[DataMember(Name = "savedate")]
		public DateTime SaveDate { get; set; }
		[DataMember(Name = "review")]
		public string Review { get; set; }
		[DataMember(Name = "rating")]
		public double Rating { get; set; }

		public EntFeedback(int Id, string EntName, string EntUsername, string SeekerName, string SeekerUsername, int Submissionid, DateTime SaveDate, string Review, double Rating)
		{
			this.Id = Id;
			this.EntName = EntName;
			this.EntUsername = EntUsername;
			this.SeekerName = SeekerName;
			this.SeekerUsername = SeekerUsername;
			this.Submissionid = Submissionid;
			this.SaveDate = SaveDate;
			this.Review = Review;
			this.Rating = Rating;
		}
	}
}
