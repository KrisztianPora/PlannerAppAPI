using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Models
{
    public class ToDoItemRequest
    {
        public string Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Description { get; set; }

        public DateTime? EstimatedDate { get; set; }

        [Required]
        public string PlanId { get; set; }
    }
}
