using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HtmlAgilityPack;
using NetPing.Models;
using NetPing_modern.ViewModels;
using WebGrease.Css.Extensions;

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
                                                                        var html = GetText(p);
                                                                        var icon = GetFirstImg(p);
                                                                        if (icon != null)
                                                                        {
                                                                            html.DocumentNode.ChildNodes.Insert(0, icon);
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
                                                                                           .Name,
                                                                                        Path =
                                                                                            p
                                                                                            .Category
                                                                                            .Path
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

        private HtmlNode GetFirstImg(Post p)
        {
            var html = new HtmlDocument();
            html.LoadHtml(p.Body);
            var img = html.DocumentNode.SelectSingleNode("//p/descendant::img");
            if (img != null)
            {
                var imgHtml = new HtmlDocument();
                // we need to remove "height" attribute to avoid violation of the proportions
                img.Attributes.Remove("height");
                // add wrapper class="img-preview"
                imgHtml.LoadHtml("<p class=\"img-preview\">" + img.ParentNode.OuterHtml + "</p>");
                return imgHtml.DocumentNode;
            }
            return null;
        }

        private static HtmlDocument GetText(Post p)
        {
            var html = new HtmlDocument();
            html.LoadHtml(p.Body);
            // usually these nodes used as a text container
            var pNodes = html.DocumentNode.SelectNodes("//p | //h1 | //ul | //h2");

            var textNodes = new HtmlNodeCollection(html.DocumentNode);

            foreach (var textNode in pNodes)
            {
                // let's ignore image nodes here
                var imgSpans = textNode.SelectNodes("descendant::span[contains(@class,'confluence-embedded-file-wrapper')]");
                if (imgSpans != null && imgSpans.Count > 0)
                    continue;

                textNodes.Append(textNode);
            }

            

            html.DocumentNode.RemoveAllChildren();

            // 4 - is the approximate number of empirically chosen to avlid too complex logic

            const int amountOfTextNodes = 4;
            textNodes.Take(amountOfTextNodes).ForEach(n => html.DocumentNode.AppendChild(n));
            return html;
        }
    }
}