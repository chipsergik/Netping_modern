using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HtmlAgilityPack;
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
        }
    }
}