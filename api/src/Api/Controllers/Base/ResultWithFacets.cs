using System.Collections.Generic;

namespace Foundatio.Skeleton.Api.Controllers.Base
{
    public class ResultWithFacets<T>
    {
        public IReadOnlyCollection<T> Results { get; set; }

        //public IReadOnlyCollection<AggregationResult> Facets { get; set; }
    }
}