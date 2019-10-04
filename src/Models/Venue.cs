using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Meetup.Models
{
	public class Venue
	{
		[JsonProperty("country")]
		public string Country { get; set; }

		[JsonProperty("localized_country_name")]
		public string LocalizedCountryName { get; set; }

		[JsonProperty("city")]
		public string City { get; set; }

		[JsonProperty("address_1")]
		public string Address1 { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("lon")]
		public double Lon { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("lat")]
		public double Lat { get; set; }

		[JsonProperty("repinned")]
		public bool Repinned { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("zip")]
		public string Zip { get; set; }

		[JsonProperty("phone")]
		public string Phone { get; set; }

		[JsonProperty("address_2")]
		public string Address2 { get; set; }
	}
}