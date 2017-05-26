﻿using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Foundatio.Skeleton.Api.Utility.Results {
    public class OkWithHeadersContentResult<T> : OkNegotiatedContentResult<T> {
        public OkWithHeadersContentResult(T content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters) : base(content, contentNegotiator, request, formatters) { }

        public OkWithHeadersContentResult(T content, ApiController controller, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = null)
            : base(content, controller) {
            Headers = headers;
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }

        public override async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
            HttpResponseMessage response = await base.ExecuteAsync(cancellationToken);

            if (Headers != null)
                foreach (var header in Headers)
                    response.Headers.Add(header.Key, header.Value);

            return response;
        }
    }

    public class OkWithResourceLinks<TContent, TEntity> : OkWithHeadersContentResult<TContent> 
        where TContent : class
        where TEntity : class {
        public OkWithResourceLinks(TContent content, IContentNegotiator contentNegotiator, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters) : 
            base(content, contentNegotiator, request, formatters) { }

        public OkWithResourceLinks(TContent content, ApiController controller, bool hasMore, int? page = null, Func<TEntity, string> pagePropertyAccessor = null, Func<TContent, IReadOnlyCollection<TEntity>> collectionAccessor = null, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = null, bool isDescending = false)
            : this(content, controller, hasMore, page, null, pagePropertyAccessor, collectionAccessor, headers, isDescending)
        {
        }

        public OkWithResourceLinks(TContent content, ApiController controller, bool hasMore, int? page = null, long? total = null, Func<TEntity, string> pagePropertyAccessor = null, Func<TContent, IReadOnlyCollection<TEntity>> collectionAccessor = null,  IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = null, bool isDescending = false) : base(content, controller) {
            if (content == null)
                return;

            List<string> links;
            if (page.HasValue)
                links = GetPagedLinks(Request.RequestUri, page.Value, hasMore);
            else
            {
                IReadOnlyCollection<TEntity> collection = null;
                if (collectionAccessor != null)
                {
                    collection = collectionAccessor(content);
                }
                
                if (collection == null)
                {
                    collection = content as IReadOnlyCollection<TEntity>;
                }

                links = GetBeforeAndAfterLinks(Request.RequestUri, collection, isDescending, hasMore, pagePropertyAccessor);
            }

            var headerItems = new Dictionary<string, IEnumerable<string>>();
            if (links.Count > 0)
                headerItems.Add("Link", links.ToArray());

            if (total.HasValue)
                headerItems.Add("X-Result-Count", new[] { total.ToString() });

            if (headers != null)
                foreach (var header in headers)
                    headerItems.Add(header.Key, header.Value);

            Headers = headerItems;
        }

        public static List<string> GetPagedLinks(Uri url, int page, bool hasMore) {
            bool includePrevious = page > 1;
            bool includeNext = hasMore;

            var previousParameters = url.ParseQueryString();
            previousParameters["page"] = (page - 1).ToString();
            var nextParameters = new NameValueCollection(previousParameters);
            nextParameters["page"] = (page + 1).ToString();

            string baseUrl = url.ToString();
            if (!String.IsNullOrEmpty(url.Query))
                baseUrl = baseUrl.Replace(url.Query, "");

            string previousLink = String.Format("<{0}?{1}>; rel=\"previous\"", baseUrl, previousParameters.ToQueryString());
            string nextLink = String.Format("<{0}?{1}>; rel=\"next\"", baseUrl, nextParameters.ToQueryString());

            var links = new List<string>();
            if (includePrevious)
                links.Add(previousLink);
            if (includeNext)
                links.Add(nextLink);

            return links;
        }

        public static List<string> GetBeforeAndAfterLinks(Uri url, IReadOnlyCollection<TEntity> content, bool isDescending, bool hasMore, Func<TEntity, string> pagePropertyAccessor) {
            if (pagePropertyAccessor == null && typeof(IIdentity).IsAssignableFrom(typeof(TEntity)))
                pagePropertyAccessor = e => ((IIdentity)e).Id;

            if (pagePropertyAccessor == null)
                return new List<string>();

            string firstId = content.Any() ? pagePropertyAccessor(!isDescending ? content.First() : content.Last()) : String.Empty;
            string lastId = content.Any() ? pagePropertyAccessor(!isDescending ? content.Last() : content.First()) : String.Empty;

            bool hasBefore = false;
            bool hasAfter = false;

            var previousParameters = url.ParseQueryString();
            if (previousParameters["before"] != null)
                hasBefore = true;
            previousParameters.Remove("before");
            if (previousParameters["after"] != null)
                hasAfter = true;
            previousParameters.Remove("after");
            var nextParameters = new NameValueCollection(previousParameters);

            previousParameters.Add("before", firstId);
            nextParameters.Add("after", lastId);

            bool includePrevious = hasBefore ? hasMore : true;
            bool includeNext = !hasBefore ? hasMore : true;
            if (hasBefore && !content.Any()) {
                // are we currently before the first page?
                includePrevious = false;
                includeNext = true;
                nextParameters.Remove("after");
            } else if (!hasBefore && !hasAfter) {
                // are we at the first page?
                includePrevious = false;
            }

            string baseUrl = url.ToString();
            if (!String.IsNullOrEmpty(url.Query))
                baseUrl = baseUrl.Replace(url.Query, "");

            string previousLink = String.Format("<{0}?{1}>; rel=\"previous\"", baseUrl, previousParameters.ToQueryString());
            string nextLink = String.Format("<{0}?{1}>; rel=\"next\"", baseUrl, nextParameters.ToQueryString());

            var links = new List<string>();
            if (includePrevious)
                links.Add(previousLink);
            if (includeNext)
                links.Add(nextLink);

            return links;
        }
    }
}
