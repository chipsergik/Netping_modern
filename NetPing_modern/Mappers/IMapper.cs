namespace NetPing_modern.Mappers
{
    public interface IMapper<TSource, TDestination>
    {
        TDestination Map(TSource source);
    }
}
