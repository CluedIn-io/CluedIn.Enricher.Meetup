// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeetupModel.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the MeetupModel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.Meetup.Models
{
    public class MeetupResponse
    {
        [JsonProperty("results")]
        public List<Event> Results { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
