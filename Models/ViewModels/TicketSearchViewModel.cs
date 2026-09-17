using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrainTickets.Models.ViewModels
{
    public class TicketSearchViewModel
    {
        public int? StartStationId { get; set; }
        public int? EndStationId { get; set; }
        public DateTime? Date { get; set; }

        public List<SelectListItem> Stations { get; set; } = new();
        public IEnumerable<TicketIndexViewModel> Trips { get; set; } = new List<TicketIndexViewModel>();
    }
}
