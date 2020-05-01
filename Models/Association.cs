using System;
using System.ComponentModel.DataAnnotations;

namespace ActivityCenter.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public User Guest { get; set; }
        public Activitys Activity { get; set; }

    }
}