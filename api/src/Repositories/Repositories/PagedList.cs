using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundatio.Skeleton.Repositories.Repositories {
    /// <summary>
    /// Paged list
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    [Serializable]
    public class PagedList<T> : List<T> {

       
        public int PageIndex { get;  set; }
        public int PageSize { get;  set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage {
            get { return (PageIndex > 0); }
        }
        public bool HasNextPage {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
