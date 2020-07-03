using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerAppAPI.Models
{
    public class Plan : Record
    {
        public Plan()
        {
            ToDoItems = new List<ToDoItem>();
        }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [StringLength(256)]
        public string CoverPath { get; set; }

        public ICollection<ToDoItem> ToDoItems { get; set; }
    }
}
