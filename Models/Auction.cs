using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models {
    public class Auction : BaseEntity {
        [Key]
        public int Id { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public float Bid { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
