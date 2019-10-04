using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Meetup.Models
{
	public class Group
	{
		[JsonProperty("join_mode")]
		public string JoinMode { get; set; }

		[JsonProperty("created")]
		public object Created { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("group_lon")]
		public double GroupLon { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("urlname")]
		public string UrlName { get; set; }

		[JsonProperty("group_lat")]
		public double GroupLat { get; set; }

		[JsonProperty("who")]
		public string Who { get; set; }
	}
}