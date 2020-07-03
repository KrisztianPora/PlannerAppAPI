using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Models
{
    public class Record
    {
        public Record()
        {
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow;
        }

        [Key]
        public string Id { get; set; }

        [JsonIgnore]
        public DateTime CreateDate { get; set; }

        [JsonIgnore]
        public DateTime ModifiedDate { get; set; }

        [Required]
        [StringLength(40)]
        [JsonIgnore]
        public string UserId { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
