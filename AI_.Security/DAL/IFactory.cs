namespace AI_.Security.DAL
{
    public interface IFactory<T>
    {
        T GetInstance();
    }
}