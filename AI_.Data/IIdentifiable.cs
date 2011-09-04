namespace AI_.Data
{
    public interface IIdentifiable<out T>
    {
        T ID { get; }
    }
}