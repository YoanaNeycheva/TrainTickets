using System.ComponentModel.DataAnnotations;

namespace TrainTickets.Enums
{
    public enum TrainType
    {
        [Display(Name = "Бърз")]
        Fast,

        [Display(Name = "Пътнически")]
        Passenger
    }
}