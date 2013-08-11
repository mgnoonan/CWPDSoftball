using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softball.Mvc4.Helpers;

namespace Softball.Mvc4.Controllers
{ 
    public class TeamsController : _Controller
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public TeamsController() : this(new Repository()) { }
        public TeamsController(IRepository repository) { _repository = repository; }

        //
        // GET: /Teams/

        public ViewResult Index()
        {
            var list = _repository.FindTeams().ToList();
            return View(list);
        }

        //
        // GET: /Teams/Details/5
        /*
        public ViewResult Details(int id)
        {
            Team team = _repository.FindTeams().Single(t => t.TeamID == id);
            return View(team);
        }

        //
        // GET: /Teams/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Teams/Create

        [HttpPost]
        public ActionResult Create(Team team)
        {
            if (ModelState.IsValid)
            {
                _repository.AddObject(team);
                _repository.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(team);
        }
        */
        //
        // GET: /Teams/Edit/5
 
        [Authorize]
        public ActionResult Edit(int id)
        {
            Team team = _repository.FindTeams().Single(t => t.TeamID == id);
            return View(team);
        }

        //
        // POST: /Teams/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Team team)
        {
            if (ModelState.IsValid)
            {
                _repository.Attach(team);
                _repository.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        //
        // GET: /Teams/Delete/5
 /*
        public ActionResult Delete(int id)
        {
            Team team = _repository.FindTeams().Single(t => t.TeamID == id);
            return View(team);
        }

        //
        // POST: /Teams/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Team team = _repository.FindTeams().Single(t => t.TeamID == id);
            _repository.DeleteObject(team);
            _repository.SaveChanges();
            return RedirectToAction("Index");
        }
        */
        protected override void Dispose(bool disposing)
        {
            _repository.Dispose();
            base.Dispose(disposing);
        }
    }
}