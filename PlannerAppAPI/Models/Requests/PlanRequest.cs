using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Models
{
    public class PlanRequest
    {
        public string Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public IFormFile CoverFile { get; set; }
    }
}
