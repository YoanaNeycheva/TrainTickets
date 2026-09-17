using System.ComponentModel.DataAnnotations;

namespace TrainTickets.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public int TripId { get; set; }
        public Trip? Trip { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Range(1, 10)]
        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        public DateTime PurchaseDate { get; set; }
    }
}
