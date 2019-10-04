// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeetupEventVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the MeetupEventVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.Meetup.Vocabularies
{
    /// <summary>The meetup event vocabulary.</summary>
    /// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
    public class MeetupEventVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeetupEventVocabulary"/> class.
        /// </summary>
        public MeetupEventVocabulary()
        {
            this.VocabularyName = "Meetup Event";
            this.KeyPrefix = "meetup.event";
            this.KeySeparator = ".";
            this.Grouping = EntityType.Calendar.Event;

            this.AddGroup("Meetup Details", group =>
            {
                this.UtcOffset = group.Add(new VocabularyKey("utcOffset", VocabularyKeyDataType.Integer, VocabularyKeyVisibility.Hidden));
                this.Venue = group.Add(new VocabularyKey("venue"));
                this.Headcount = group.Add(new VocabularyKey("headcount", VocabularyKeyDataType.Integer));
                this.Visibility = group.Add(new VocabularyKey("visibility"));
                this.WaitlistCount = group.Add(new VocabularyKey("waitlistCount", VocabularyKeyDataType.Integer));
                this.Created = group.Add(new VocabularyKey("created", VocabularyKeyDataType.Time));
                this.MaybeRsvpCount = group.Add(new VocabularyKey("maybeRsvpCount"));
                this.Description = group.Add(new VocabularyKey("description", VocabularyKeyDataType.Html));
                this.EventUrl = group.Add(new VocabularyKey("eventUrl", VocabularyKeyDataType.Uri));
                this.YesRsvpCount = group.Add(new VocabularyKey("yesRsvpCount", VocabularyKeyDataType.Integer));
                this.Duration = group.Add(new VocabularyKey("duration", VocabularyKeyDataType.Duration));
                this.Name = group.Add(new VocabularyKey("name"));
                this.Id = group.Add(new VocabularyKey("id", VocabularyKeyDataType.Integer, VocabularyKeyVisibility.Hidden));
                this.Time = group.Add(new VocabularyKey("time", VocabularyKeyDataType.Time));
                this.Updated = group.Add(new VocabularyKey("updated", VocabularyKeyDataType.Time));
                this.Group = group.Add(new VocabularyKey("group"));
                this.Status = group.Add(new VocabularyKey("status"));
                this.HowToFindUs = group.Add(new VocabularyKey("howToFindUs"));
                this.RsvpLimit = group.Add(new VocabularyKey("rsvpLimit", VocabularyKeyDataType.Integer));
                this.Fee = group.Add(new VocabularyKey("fee"));
            });

            this.AddGroup("Venue Details", group =>
            {
                this.VenueAddress1 = group.Add(new VocabularyKey("venueAddress1"));
                this.VenueAddress2 = group.Add(new VocabularyKey("venueAddress2"));
                this.VenueCity = group.Add(new VocabularyKey("venueCity"));
                this.VenueCountry = group.Add(new VocabularyKey("venueCountry"));
                this.VenueLat = group.Add(new VocabularyKey("venueLat"));
                this.VenueLocalizedCountryName = group.Add(new VocabularyKey("venueLocalizedCountryName"));
                this.VenueLon = group.Add(new VocabularyKey("venueLon"));
                this.VenueName = group.Add(new VocabularyKey("venueName"));
                this.VenuePhone = group.Add(new VocabularyKey("venuePhone"));
                this.VenueRepinned = group.Add(new VocabularyKey("venueRepinned"));
                this.VenueState = group.Add(new VocabularyKey("venueState"));
                this.VenueZip = group.Add(new VocabularyKey("venueZip"));

            });

            this.AddGroup("Group Details", group =>
            {
                this.GroupCreated = group.Add(new VocabularyKey("groupCreated"));
                this.GroupLat = group.Add(new VocabularyKey("groupLat"));
                this.GroupLon = group.Add(new VocabularyKey("groupLon"));
                this.GroupJoinMode = group.Add(new VocabularyKey("groupJoinMode"));
            });

            this.AddGroup("Fee Details", group =>
            {
                this.FeeAccepts = group.Add(new VocabularyKey("feeAccepts"));
                this.FeeAmount = group.Add(new VocabularyKey("feeAmount"));
                this.FeeCurrency = group.Add(new VocabularyKey("feeCurrency"));
                this.FeeDescription = group.Add(new VocabularyKey("feeDescription"));
                this.FeeLabel = group.Add(new VocabularyKey("feeLabel"));
                this.FeeRequired = group.Add(new VocabularyKey("feeRequired"));
            });

            this.AddMapping(this.VenueAddress1, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInEvent.Location);
            this.AddMapping(this.Time, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInEvent.Start);
            this.AddMapping(this.Created, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInDates.CreatedDate);
            this.AddMapping(this.Updated, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInDates.ModifiedDate);
        }

        public VocabularyKey UtcOffset { get; private set; }
        public VocabularyKey Venue { get; private set; }
        public VocabularyKey Headcount { get; internal set; }
        public VocabularyKey Visibility { get; internal set; }
        public VocabularyKey WaitlistCount { get; internal set; }
        public VocabularyKey Created { get; internal set; }
        public VocabularyKey MaybeRsvpCount { get; internal set; }
        public VocabularyKey Description { get; internal set; }
        public VocabularyKey EventUrl { get; internal set; }
        public VocabularyKey YesRsvpCount { get; internal set; }
        public VocabularyKey Duration { get; internal set; }
        public VocabularyKey Name { get; internal set; }
        public VocabularyKey Id { get; internal set; }
        public VocabularyKey Time { get; internal set; }
        public VocabularyKey Updated { get; internal set; }
        public VocabularyKey Group { get; internal set; }
        public VocabularyKey Status { get; internal set; }
        public VocabularyKey HowToFindUs { get; internal set; }
        public VocabularyKey RsvpLimit { get; internal set; }
        public VocabularyKey Fee { get; internal set; }

        public VocabularyKey VenueAddress1 { get; internal set; }
        public VocabularyKey VenueAddress2 { get; internal set; }
        public VocabularyKey VenueCity { get; internal set; }
        public VocabularyKey VenueCountry { get; internal set; }
        public VocabularyKey VenueLat { get; internal set; }
        public VocabularyKey VenueLocalizedCountryName { get; internal set; }
        public VocabularyKey VenueLon { get; internal set; }
        public VocabularyKey VenueName { get; internal set; }
        public VocabularyKey VenuePhone { get; internal set; }
        public VocabularyKey VenueRepinned { get; internal set; }
        public VocabularyKey VenueState { get; internal set; }
        public VocabularyKey VenueZip { get; internal set; }

        public VocabularyKey GroupCreated { get; internal set; }
        public VocabularyKey GroupLat { get; internal set; }
        public VocabularyKey GroupLon { get; internal set; }
        public VocabularyKey GroupJoinMode { get; internal set; }

        public VocabularyKey FeeAccepts { get; internal set; }
        public VocabularyKey FeeAmount { get; internal set; }
        public VocabularyKey FeeCurrency { get; internal set; }
        public VocabularyKey FeeDescription { get; internal set; }
        public VocabularyKey FeeLabel { get; internal set; }
        public VocabularyKey FeeRequired { get; internal set; }
    }
}
