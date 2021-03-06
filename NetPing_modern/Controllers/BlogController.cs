﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NetPing.DAL;
using NetPing.Models;
using NetPing_modern.Mappers;
using NetPing_modern.ViewModels;
using WebGrease.Css.Extensions;

namespace NetPing_modern.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository _repository;
        private readonly IMapper<Post, PostViewModel> _postMapper;
        private readonly IMapper<SPTerm, TermViewModel> _termMapper;
        private readonly IMapper<SPTerm, CategoryViewModel> _categoryMapper; 

        public BlogController(IRepository repository, 
            IMapper<Post,PostViewModel> postMapper, 
            IMapper<SPTerm, TermViewModel> termMapper,
            IMapper<SPTerm, CategoryViewModel> categoryMapper)
        {
            _repository = repository;
            _postMapper = postMapper;
            _termMapper = termMapper;
            _categoryMapper = categoryMapper;
        }

        public ActionResult Main(List<string> tags = null)
        {
            
            List<Post> posts;
            Dictionary<Guid, SPTerm> devices;
            var model = CreateModel(out posts, out devices);
            model.Tags.ForEach(t =>
                               {
                                   if (tags == null)
                                       return;
                                   if (tags.Contains(t.Path))
                                   {
                                       t.IsSelected = true;
                                   }
                               });

            model.Posts.ForEach(p => p.Tags.ForEach(t =>
                                                    {
                                                        if (tags == null)
                                                            return;
                                                        if (tags.Contains(t.Path))
                                                        {
                                                            t.IsSelected = true;
                                                        }
                                                    }));

            if (tags != null && tags.Count > 0)
            {
                model.Posts = model.Posts.Where(m => m.Tags.Any(t => tags.Contains(t.Path))).ToList();
            }

            if (!model.Posts.Any())
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Блог компании Netping, новости, примеры применения, ответы на вопросы";
            ViewBag.Description = "Блог компании Netping, новости компании Netping, примеры применения устройств Netping, ответы на вопросы об устройствах Netping";
            ViewBag.Keys = "Netping блог, новости Netping, ответы на вопросы Netping, примеры применения Netping";

            ViewBag.BlogCategoryName = "";

            return View(model);
        }

        public ActionResult Search(string q, List<string> tags = null)
        {
            List<Post> posts;
            Dictionary<Guid, SPTerm> devices;
            var model = CreateModel(out posts, out devices);
            model.Query = q;
            model.Posts = posts.Where(p => p.Body.Contains(q)).OrderByDescending(item => item.Created).Select(_postMapper.Map).ToList();
            if (tags != null && tags.Count > 0)
            {
                model.Posts = model.Posts.Where(m => m.Tags.Any(t => tags.Contains(t.Path))).ToList();
            }
            model.Tags.ForEach(t =>
            {
                if (tags == null)
                    return;
                if (tags.Contains(t.Path))
                {
                    t.IsSelected = true;
                }
            });

            if (!model.Posts.Any())
            {
                return View("NoPosts",model);
            }

            model.Posts.ForEach(p => p.Tags.ForEach(t =>
            {
                if (tags == null)
                    return;
                if (tags.Contains(t.Path))
                {
                    t.IsSelected = true;
                }
            }));

            ViewBag.Title = "Поиск в блоге: "+q;
            ViewBag.BlogCategory = "";


            return View("Main", model);
        }

        private BlogViewModel CreateModel(out List<Post> posts, out Dictionary<Guid, SPTerm> devices)
        {
            var model = new BlogViewModel();

            posts = _repository.Posts.ToList();
            model.TopPosts = posts.OrderByDescending(item => item.Created).Where(item => item.IsTop).Select(_postMapper.Map).ToList();
            model.Posts = posts.OrderByDescending(item => item.Created).Select(_postMapper.Map).ToList();
            model.Categories = _repository.TermsCategories.Select(_categoryMapper.Map).ToList();

            devices = new Dictionary<Guid, SPTerm>();
            foreach (var post in posts)
            {
                foreach (var device in post.Devices)
                {
                    if (!devices.ContainsKey(device.Id))
                    {
                        devices.Add(device.Id, device);
                    }
                }
            }
            model.Tags = devices.Values.Select(_termMapper.Map).Select(t =>
                                                                       {
                                                                           var m = new TagViewModel
                                                                                   {
                                                                                       Id = t.Id,
                                                                                       Name = t.Name,
                                                                                       Path = t.Path
                                                                                   };
                                                                           return m;
                                                                       }).ToList();
            return model;
        }
        
        public ActionResult Record(string postname)
        {
            List<Post> posts;
            Dictionary<Guid, SPTerm> devices;
            var model = CreateModel(out posts, out devices);
            model.Post = _postMapper.Map(posts.FirstOrDefault(item => item.Url_name == string.Format("/Blog/{0}", postname)));

            if (model.Post == null) return HttpNotFound();

            ViewBag.Title = model.Post.Title;
            ViewBag.Description = model.Post.ShortBody;

            ViewBag.BlogCategoryName = model.Post.Category.Name;
            ViewBag.BlogCategoryPath = model.Post.Category.Path;

            return View(model);
        }
        /*
        public ActionResult Post(int id)
        {
            List<Post> posts;
            Dictionary<Guid, SPTerm> devices;
            var model = CreateModel(out posts, out devices);
            model.Post = _postMapper.Map(posts.FirstOrDefault(item => item.Id == id));

            ViewBag.Title = model.Post.Title;
            ViewBag.Description = model.Post.ShortBody;

            return View("Record", model);
        }
        */
        public ActionResult Category(string path, List<string> tags = null)
        {
            List<Post> posts;
            Dictionary<Guid, SPTerm> devices;
            var model = CreateModel(out posts, out devices);
            model.Posts = posts.Where(p => p.Category.Path == path).OrderByDescending(item => item.Created).Select(_postMapper.Map).ToList();
            if (tags != null && tags.Count > 0)
            {
                model.Posts = model.Posts.Where(m => m.Tags.Any(t => tags.Contains(t.Path))).ToList();
            }
            model.Tags.ForEach(t =>
            {
                if (tags == null)
                    return;
                if (tags.Contains(t.Path))
                {
                    t.IsSelected = true;
                }
            });

            model.Categories.ForEach(c => c.IsSelected = c.Path == path);

            if (!model.Posts.Any())
            {
                return HttpNotFound();
            }

            model.Posts.ForEach(p => p.Tags.ForEach(t =>
            {
                if (tags == null)
                    return;
                if (tags.Contains(t.Path))
                {
                    t.IsSelected = true;
                }
            }));

            if (path == "FAQ")
            {
                ViewBag.Title = "Ответы на вопросы про устройства Netping";
                ViewBag.Description = "Ответы на типовые вопросы про использование устройств Netping";
                ViewBag.Keys = "ответы на вопросы";
            }
            if (path == "News")
            {

                ViewBag.Title = "Новости компании Netping";
                ViewBag.Description = "Периодически публикуемые новости о компании Netping, обновления ПО устройств, выпуск новых устройств";
                ViewBag.Keys = "новости Netping, обновление прошивок, выпуск новых устройств";
            }
            if (path == "Tutorial")
            {
                ViewBag.Title = "Примеры применения устройств Netping";
                ViewBag.Description = "Примеры применения, типовые схемы использования устройств Netping";
                ViewBag.Keys = "примеры применения, типовые схемы";
            }

            ViewBag.BlogCategoryName = model.Categories.FirstOrDefault(c => c.IsSelected).Name;
            ViewBag.BlogCategoryPath = model.Categories.FirstOrDefault(c => c.IsSelected).Path;

            return View("Main", model);
        }
	}
}