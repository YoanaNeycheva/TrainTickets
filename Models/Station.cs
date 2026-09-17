using System.ComponentModel.DataAnnotations;

namespace TrainTickets.Models
{
    public class Station
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Моля въведете име на гарата.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Моля въведете град.")]
        public string City { get; set; }
    }
}
