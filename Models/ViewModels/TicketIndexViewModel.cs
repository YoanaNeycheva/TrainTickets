using TrainTickets.Enums;

namespace TrainTickets.Models.ViewModels
{
    public class TicketIndexViewModel
    {
        public int Id { get; set; }
        public string TrainName { get; set; }

        public string StartStation { get; set; }
        public string EndStation { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        public int Capacity { get; set; }
        public int SoldSeats { get; set; }

        public int FreeSeats => Capacity - SoldSeats;
        public TrainType TrainType { get; set; }
    }
}
