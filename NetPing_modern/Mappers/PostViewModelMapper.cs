﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HtmlAgilityPack;
using NetPing.Models;
using NetPing_modern.ViewModels;

namespace NetPing_modern.Mappers
{
    public class PostViewModelMapper : DefaultMapper<Post,PostViewModel>
    {
        private readonly IMapper<SPTerm, TermViewModel> _termMapper;

        public PostViewModelMapper(IMapper<SPTerm, TermViewModel> termMapper)
        {
            _termMapper = termMapper;
        }

        protected override void Configure(IMappingExpression<Post, PostViewModel> mapping)
        {
            mapping.ForMember(m => m.ShortBody, o => o.ResolveUsing(p =>
                                                               {
                                                                   var html = new HtmlDocument();
                                                                   html.LoadHtml(p.Body);
                                                                   var pNodes = html.DocumentNode.SelectNodes("//p");
                                                                   while (pNodes.Count > 1)
                                                                   {
                                                                       var current = pNodes[pNodes.Count - 1];
                                                                       current.Remove();
                                                                       pNodes.Remove(current);
                                                                   }

                                                                   var pNode = pNodes.FirstOrDefault();
                                                                   if (pNode != null)
                                                                   {
                                                                       var parentNode = pNode.ParentNode;
                                                                       var pNodeHasTreated = false;
                                                                       var removedNodes = new List<HtmlNode>();
                                                                       foreach (var childNode in parentNode.ChildNodes)
                                                                       {
                                                                           if (!pNodeHasTreated && childNode == pNode)
                                                                           {
                                                                               pNodeHasTreated = true;
                                                                               continue;
                                                                           }

                                                                           if (pNodeHasTreated)
                                                                           {
                                                                               removedNodes.Add(childNode);
                                                                           }
                                                                       }
                                                                       removedNodes.ForEach(n => n.Remove());
                                                                   }

                                                                   return html.DocumentNode.InnerHtml;
                                                               }));

            mapping.ForMember(m => m.Url, o => o.ResolveUsing(p =>
                                                              {
                                                                  if (!string.IsNullOrEmpty(p.Url_name))
                                                                  {
                                                                      return p.Url_name;
                                                                  }

                                                                  if (p.Id != 0)
                                                                  {
                                                                      return string.Format("/view.aspx?id={0}", p.Id);
                                                                  }

                                                                  return "#";
                                                              }));

            mapping.ForMember(m => m.Category, o => o.ResolveUsing(p =>
                                                                   {
                                                                       var model = new TermViewModel
                                                                                   {
                                                                                       Id =
                                                                                           p
                                                                                           .Category
                                                                                           .Id,
                                                                                       Name =
                                                                                           p
                                                                                           .Category
                                                                                           .Name
                                                                                   };
                                                                       return model;
                                                                   }));
            mapping.ForMember(m => m.Tags, o => o.ResolveUsing(p => p.Devices.Select(_termMapper.Map).Select(t =>
                                                                                                             {
                                                                                                                 var m = new TagViewModel
                                                                                                                         {
                                                                                                                             Id = t.Id,
                                                                                                                             Name = t.Name,
                                                                                                                             Path = t.Path
                                                                                                                         };
                                                                                                                 return m;
                                                                                                             })));
        }
    }
}