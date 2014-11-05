using AutoMapper;

namespace NetPing_modern.Mappers
{
    public class DefaultMapper<TSource, TDestination> : Profile, IMapper<TSource, TDestination>
    {
        public TDestination Map(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        protected override void Configure()
        {
            var mappingExpression = CreateMap<TSource, TDestination>();
            Configure(mappingExpression);
        }

        protected virtual void Configure(IMappingExpression<TSource, TDestination> mapping)
        {
            
        }
    }
}