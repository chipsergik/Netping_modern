using System;
using AutoMapper;
using NetPing.Models;
using NetPing_modern.ViewModels;

namespace NetPing_modern.Mappers
{
    public class PostViewModelMapper : DefaultMapper<Post,PostViewModel>
    {
        protected override void Configure(IMappingExpression<Post, PostViewModel> mapping)
        {
            mapping.ForMember(m => m.Body, o => o.ResolveUsing(p =>
                                                               {
                                                                   const string endTag = "</p>";
                                                                   var index = p.Body.IndexOf(endTag,
                                                                       StringComparison.InvariantCultureIgnoreCase);
                                                                   if (index == -1)
                                                                       return p.Body;

                                                                   return p.Body.Substring(0, index + endTag.Length);
                                                               }));
        }
    }
}