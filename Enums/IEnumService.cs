namespace TrainTickets.Enums
{
    public interface IEnumService<T>
    {
        IEnumerable<EnumDTO> GetAll();
    }
}
