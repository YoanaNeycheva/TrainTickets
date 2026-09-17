using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TrainTickets.Models.ViewModels
{
    public class TripViewModel
    {
        public int Id { get; set; }
        [Required]
        public int TrainId { get; set; }

        [Required]
        public int StartStationId { get; set; }

        [Required]
        public int EndStationId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public double Price { get; set; }

        public IEnumerable<SelectListItem>? Trains { get; set; }
        public IEnumerable<SelectListItem>? Stations { get; set; }
    }
}
