using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Xml.Schema;
using ToDoDemo.Models;

namespace ToDoDemo.Controllers
{
    public class HomeController : Controller
    {
        private ToDoContext context;

        public HomeController(ToDoContext ctx) => context = ctx;

        public IActionResult Index(string? id)
        {
            var filters = new Filters(id);


            // Created a list of KeyValuePair
            List<KeyValuePair<string, string>> DueFilter = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("future", "Future"),
                    new KeyValuePair<string, string>("past","Past"),
                    new KeyValuePair<string, string>("today","Today")
                };

            //binding the list value
            IEnumerable<SelectListItem> selectListItems = DueFilter.Select(i => new SelectListItem
            {
                Value = i.Key,
                Text = i.Value
            });

            SelectList selectList = new SelectList(selectListItems, "Value", "Text");
            //stores the vlaue of selectlist

            ViewBag.DueFilters = selectList;

            filters.DueFilterValue = DueFilter;
            ViewBag.Filters = filters;

            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();


            IQueryable<ToDo> query = context.ToDOs
                    .Include(t => t.Category)
                    .Include(t => t.Status);

            if (filters.HasCategory)
            {
                query = query.Where(t => t.CategoryId == filters.CategoryId);
            }

            if (filters.HasStatus)
            {
                query = query.Where(t => t.StatusId == filters.StatusId);
            }
            else
            {
                query = query.Where(t => t.StatusId == "open");
            }

            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);

                }
            }
            var tasks = query.OrderBy(t => t.DueDate).ToList();
            //var tasks = query.Where(x => x.Status.StatusId != "closed").OrderBy(t => t.DueDate).ToList();

            return View(tasks);

        }

        [HttpGet]

        public IActionResult Add()
        {
            ViewBag.Categories = context.Categories.ToList();
            var task = new ToDo { StatusId = "open" };
            return View(task);
        }

        [HttpPost]

        public IActionResult Add(ToDo task)
        {
            if (ModelState.IsValid)
            {
                context.ToDOs.Add(task);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Status = context.Statuses.ToList();
                return View(task);
            }
        }
        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join('-', filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
        {
            selected = context.ToDOs.Find(selected.Id)!;

            if (selected != null)
            {
                selected.StatusId = "closed";
                context.SaveChanges();
            }
            return RedirectToAction("Index", new { ID = id });

        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            var toDelete = context.ToDOs.Where(t => t.StatusId == "closed").ToList();

            foreach (var task in toDelete)
            {
                context.ToDOs.Remove(task);
            }
            context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }
    }
}