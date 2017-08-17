using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models {
    public class Bid : BaseEntity {
        [Key]
        public int UserId { get; set; }
        public User User { get; set; }
        [Key]
        public int AuctionId {get; set; }
        public float BidAmount {get; set; }        
    }
}
