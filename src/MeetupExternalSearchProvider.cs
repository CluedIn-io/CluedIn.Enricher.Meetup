// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeetupExternalSearchProvider.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the MeetupExternalSearchProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.Meetup.Vocabularies;
using CluedIn.Core.ExternalSearch;

using RestSharp;
using CluedIn.Core.Utilities;
using CluedIn.ExternalSearch.Providers.Meetup.Models;

namespace CluedIn.ExternalSearch.Providers.Meetup
{
    /// <summary>The meetup external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class MeetupExternalSearchProvider : ExternalSearchProviderBase
    {
        private class TemporaryTokenProvider : IExternalSearchTokenProvider
        {
            public string ApiToken { get; private set; }

            public TemporaryTokenProvider(string token)
            {
                ApiToken = token;
            }
        }

        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetupExternalSearchProvider"/> class.
        /// </summary>
        public MeetupExternalSearchProvider()
            : base(new Guid("92d22098-c165-4f4b-b7f0-17596c98653f"), EntityType.Organization)
        {
            TokenProvider           = new TemporaryTokenProvider("636a43386b2942649206c7a6a304044");
            TokenProviderIsRequired = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeetupExternalSearchProvider"/> class.
        /// </summary>
        public MeetupExternalSearchProvider(IExternalSearchTokenProvider tokenProvider)
            : base(new Guid("92d22098-c165-4f4b-b7f0-17596c98653f"), EntityType.Organization)
        {
            TokenProvider           = tokenProvider;
            TokenProviderIsRequired = true;
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        /// <summary>Builds the queries.</summary>
        /// <param name="context">The context.</param>
        /// <param name="request">The request.</param>
        /// <returns>The search queries.</returns>
        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            var existingResults = request.GetQueryResults<Event>(this).ToList();

            Func<string, bool> nameFilter = value => OrganizationFilters.NameFilter(context, value) || existingResults.Any(r => string.Equals(r.Data.Id, value, StringComparison.InvariantCultureIgnoreCase));

            var entityType = request.EntityMetaData.EntityType;
            var organizationName = request.QueryParameters.GetValue(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());

            organizationName.Add(request.EntityMetaData.Name);
            organizationName.Add(request.EntityMetaData.DisplayName);
            organizationName.AddRange(request.EntityMetaData.Aliases);

            if (organizationName != null)
            {
                var values = organizationName.GetOrganizationNameVariants()
                    .Select(NameNormalization.Normalize)
                    .ToHashSet();

                foreach (var value in values.Where(v => !nameFilter(v)))
                {
                    yield return new ExternalSearchQuery(this, entityType, ExternalSearchQueryParameter.Name, value);
                }
            }
        }

        /// <summary>Executes the search.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <returns>The results.</returns>
        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var searchText = query.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault();

            if (string.IsNullOrEmpty(searchText))
                yield break;

            var client = new RestClient("https://api.meetup.com/2");
            var request = new RestRequest("open_events", Method.GET);
            request.AddQueryParameter("sign", "true");
            request.AddQueryParameter("key", TokenProvider.ApiToken);
            request.AddQueryParameter("status", "upcoming");
            request.AddQueryParameter("text", searchText);
            request.AddQueryParameter("and_text", "true");

            int page = 20;
            request.AddQueryParameter("page", page.ToString());
            int offset = 0;

            while (true)
            {
                request.AddQueryParameter("offset", offset.ToString());
                var response = client.ExecuteTaskAsync<MeetupResponse>(request).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Data != null)
                    {
                        foreach (var result in response.Data.Results)
                        {
                            yield return new ExternalSearchQueryResult<Event>(query, result);
                        }
                    }

                    if (response.Data.Results.Count < page)
                        break;

                    offset = offset + page;
                }
                else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                    yield break;
                else if (response.ErrorException != null)
                    throw new AggregateException(response.ErrorException.Message, response.ErrorException);
                else
                    throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
            }
        }

        /// <summary>Builds the clues.</summary>
        /// <param name="context">The context.</param>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The clues.</returns>
        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<Event>();
            var code = new EntityCode(EntityType.Calendar.Event, this.GetCodeOrigin(), resultItem.Data.Id);
            var clue = new Clue(code, context.Organization);

            this.PopulateMetadata(clue.Data.EntityData, resultItem);

            return new[] { clue };
        }

        /// <summary>Gets the primary entity metadata.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The primary entity metadata.</returns>
        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var metadata = new ProcessedEntityMetadataPart();

            this.PopulatePrimaryMetadata(metadata, request);

            return metadata;
        }

        /// <summary>Gets the preview image.</summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        /// <param name="request">The request.</param>
        /// <returns>The preview image.</returns>
        public override IPreviewImage GetPrimaryEntityPreviewImage(
            ExecutionContext context,
            IExternalSearchQueryResult result,
            IExternalSearchRequest request)
        {
            return null;
        }

        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<Event> resultItem)
        {
            return new EntityCode(EntityType.Calendar.Event, this.GetCodeOrigin(), resultItem.Data.Id);
        }

        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("meetup");
        }

        private void PopulatePrimaryMetadata(IProcessedEntityMetadata metadata, IExternalSearchRequest request)
        {
            var code = new EntityCode(EntityType.Organization, CodeOrigin.CluedIn.CreateSpecific("name"), request.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault());

            metadata.EntityType = EntityType.Organization;
            metadata.Name = request.QueryParameters[ExternalSearchQueryParameter.Name].FirstOrDefault();
            metadata.OriginEntityCode = code;
            metadata.IsShadowEntity = true;

            metadata.Codes.Add(code);
        }

        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<Event> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);

            if (metadata == null || resultItem == null)
                return;

            var input = resultItem.Data;

            metadata.EntityType = EntityType.Calendar.Event;
            metadata.Name = input.Name;
            metadata.Description = input.Description;
            metadata.OriginEntityCode = code;
            metadata.Uri = input.EventUrl != null ? new Uri(input.EventUrl) : null;

            metadata.Codes.Add(code);

            var vocab = new MeetupEventVocabulary();

            long createdDate;

            if (long.TryParse(input.Created.ToString(), out createdDate))
            {
                metadata.CreatedDate = DateUtilities.EpochRef.AddMilliseconds(createdDate);
            }
            // TODO: Missing fields - See https://www.meetup.com/meetup_api/docs/2/open_events/
            metadata.Properties[vocab.UtcOffset] = input.UtcOffset.PrintIfAvailable();

            if (input.Venue != null)
            {
                metadata.Properties[vocab.VenueAddress1] = input.Venue.Address1.PrintIfAvailable(); // TODO: Object
                metadata.Properties[vocab.VenueAddress2] = input.Venue.Address2.PrintIfAvailable();
                metadata.Properties[vocab.VenueCity] = input.Venue.City.PrintIfAvailable();
                metadata.Properties[vocab.VenueCountry] = input.Venue.Country.PrintIfAvailable();
                metadata.Properties[vocab.VenueLat] = input.Venue.Lat.PrintIfAvailable();
                metadata.Properties[vocab.VenueLocalizedCountryName] = input.Venue.LocalizedCountryName.PrintIfAvailable();
                metadata.Properties[vocab.VenueLon] = input.Venue.Lon.PrintIfAvailable();
                metadata.Properties[vocab.VenueName] = input.Venue.Name.PrintIfAvailable();
                metadata.Properties[vocab.VenuePhone] = input.Venue.Phone.PrintIfAvailable();
                metadata.Properties[vocab.VenueRepinned] = input.Venue.Repinned.PrintIfAvailable();
                metadata.Properties[vocab.VenueState] = input.Venue.State.PrintIfAvailable();
                metadata.Properties[vocab.VenueZip] = input.Venue.Zip.PrintIfAvailable();
            }

            metadata.Properties[vocab.Headcount] = input.Headcount.PrintIfAvailable();
            metadata.Properties[vocab.Visibility] = input.Visibility.PrintIfAvailable();
            metadata.Properties[vocab.WaitlistCount] = input.WaitlistCount.PrintIfAvailable();
            metadata.Properties[vocab.Created] = input.Created.PrintIfAvailable(); // TODO: Object
            metadata.Properties[vocab.MaybeRsvpCount] = input.MaybeRsvpCount.PrintIfAvailable();
            metadata.Properties[vocab.YesRsvpCount] = input.YesRsvpCount.PrintIfAvailable();
            metadata.Properties[vocab.Duration] = input.Duration.PrintIfAvailable();
            metadata.Properties[vocab.Id] = input.Id.PrintIfAvailable();
            metadata.Properties[vocab.Time] = input.Time.PrintIfAvailable(); // TODO: Object

            long modifiedDate;

            if (long.TryParse(input.Updated.ToString(), out modifiedDate))
            {
                metadata.ModifiedDate = DateUtilities.EpochRef.AddMilliseconds(modifiedDate);
            }

            metadata.Properties[vocab.Updated] = input.Updated.PrintIfAvailable(); // TODO: Object

            if (input.Group != null)
            {
                metadata.Properties[vocab.GroupCreated] = input.Group.Created.PrintIfAvailable(); // TODO: Object
                metadata.Properties[vocab.GroupLat] = input.Group.GroupLat.PrintIfAvailable(); // TODO: Object
                metadata.Properties[vocab.GroupLon] = input.Group.GroupLon.PrintIfAvailable(); // TODO: Object
                metadata.Properties[vocab.GroupJoinMode] = input.Group.JoinMode.PrintIfAvailable(); // TODO: Object
            }

            metadata.Properties[vocab.Status] = input.Status.PrintIfAvailable();
            metadata.Properties[vocab.HowToFindUs] = input.HowToFindUs.PrintIfAvailable();
            metadata.Properties[vocab.RsvpLimit] = input.RsvpLimit.PrintIfAvailable();

            if (input.Fee != null)
            {
                metadata.Properties[vocab.FeeAccepts] = input.Fee.Accepts.PrintIfAvailable(); // TODO: Object
                metadata.Properties[vocab.FeeAmount] = input.Fee.Amount.PrintIfAvailable();
                metadata.Properties[vocab.FeeCurrency] = input.Fee.Currency.PrintIfAvailable();
                metadata.Properties[vocab.FeeDescription] = input.Fee.Description.PrintIfAvailable();
                metadata.Properties[vocab.FeeLabel] = input.Fee.Label.PrintIfAvailable();
                metadata.Properties[vocab.FeeRequired] = input.Fee.Required.PrintIfAvailable();
            }
        }
    }
}