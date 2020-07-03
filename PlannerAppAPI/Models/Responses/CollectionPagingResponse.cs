using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Models.Responses
{
    public class CollectionPagingResponse<T> : CollectionResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? NextPage { get; set; }

    }
}
