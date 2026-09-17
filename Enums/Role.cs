using System.ComponentModel.DataAnnotations;

namespace TrainTickets.Enums
{
    public enum Role
    {
        [Display(Name = "Администратор")]
        Admin,

        [Display(Name = "Клиент")]
        Client
    }
}