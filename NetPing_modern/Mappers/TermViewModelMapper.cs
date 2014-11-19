using AutoMapper;
using NetPing.Models;
using NetPing_modern.ViewModels;

namespace NetPing_modern.Mappers
{
    public class TermViewModelMapper : DefaultMapper<SPTerm, TermViewModel>
    {
        protected override void Configure(IMappingExpression<SPTerm, TermViewModel> mapping)
        {
            mapping.ForMember(m => m.Id, o => o.ResolveUsing(t => t.Id));
            mapping.ForMember(m => m.Name, o => o.ResolveUsing(t => t.Name));
            mapping.ForMember(m => m.Path, o => o.ResolveUsing(t => t.Path));
        }
    }
}