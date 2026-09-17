using System;
using System.ComponentModel.DataAnnotations;

namespace TrainTickets.Models
{
    public class Trip
    {
        public int Id { get; set; }

        public int? TrainId { get; set; }

        public Train Train { get; set; }

        public int StartStationId { get; set; }
        public Station StartStation { get; set; }

        public int EndStationId { get; set; }
        public Station EndStation { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DepartureTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ArrivalTime { get; set; }

        public double Price { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
