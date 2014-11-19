using AutoMapper;
using NetPing.Models;
using NetPing_modern.ViewModels;

namespace NetPing_modern.Mappers
{
    public class CategoryViewModelMapper : DefaultMapper<SPTerm, CategoryViewModel>
    {
        protected override void Configure(IMappingExpression<SPTerm, CategoryViewModel> mapping)
        {
            mapping.ForMember(m => m.Id, o => o.ResolveUsing(t => t.Id));
            mapping.ForMember(m => m.Name, o => o.ResolveUsing(t => t.Name));
            mapping.ForMember(m => m.Path, o => o.ResolveUsing(t => t.Path));
            mapping.ForMember(m => m.IsSelected, o => o.UseValue(false));
        }
    }
}