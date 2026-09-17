using System.ComponentModel.DataAnnotations;
using TrainTickets.Enums;

namespace TrainTickets.Models
{
    public class Train
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Моля въведете име на влака.")]
        public string Name { get; set; }

        [Required]
        public TrainType Type { get; set; }

        [Required]
        public int Capacity { get; set; }

        public ICollection<Trip>? Trips { get; set; }
    }
}
