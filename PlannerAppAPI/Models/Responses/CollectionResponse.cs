using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Models.Responses
{
    public class CollectionResponse<T>
    {
        public CollectionResponse()
        {
            OperationDate = DateTime.Now;
        }

        public IEnumerable<T> Records { get; set; }

        public string Message { get; set; }
        public int Count { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime OperationDate { get; set; }
    }

}
