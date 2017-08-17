using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models {
    public class AuctionViewModel : BaseEntity {

        [Required]
        [MinLength(3)]
        public string Product { get; set; }
        [Required(ErrorMessage="Please enter a starting bid amount")]
        public float Bid { get; set; }
        [Required]
        [MinLength(10)]
        public string Description { get; set; }
        [Required(ErrorMessage="Please enter an ending date for this auction")]
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }

    }
}