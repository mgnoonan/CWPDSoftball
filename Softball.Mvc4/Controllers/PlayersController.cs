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
    [Authorize]
    public class PlayersController : Controller
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public PlayersController() : this(new Repository()) { }
        public PlayersController(IRepository repository) { _repository = repository; }

        //
        // GET: /Players/

        public ViewResult Index()
        {
            var list = _repository.FindPlayers()
                                  .Where(g => !g.LastName.Contains("Noonan"))
                                  .OrderByDescending(g => g.IsActive).ThenBy(g => g.LastName)
                                  .ToList();

            return View(list);
        }

        //
        // GET: /Players/Details/5
        /*
        public ViewResult Details(int id)
        {
            Player player = _repository.FindPlayers().Single(p => p.PlayerID == id);
            return View(player);
        }
        */
        //
        // GET: /Players/Create
        
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Players/Create

        [HttpPost]
        public ActionResult Create(Player player)
        {
            if (ModelState.IsValid)
            {
                player.IsEmailEnabled = !string.IsNullOrWhiteSpace(player.Email);
                player.IsTextEnabled = !string.IsNullOrWhiteSpace(player.TextEmail);
                player.IsActive = true;

                _repository.AddObject(player);
                _repository.SaveChanges();
                
                return RedirectToAction("Index");  
            }

            return View(player);
        }
        
        //
        // GET: /Players/Edit/5
 
        public ActionResult Edit(int id)
        {
            Player player = _repository.FindPlayers().Single(p => p.PlayerID == id);
            return View(player);
        }

        //
        // POST: /Players/Edit/5

        [HttpPost]
        public ActionResult Edit(Player player)
        {
            if (ModelState.IsValid)
            {
                player.IsEmailEnabled = !string.IsNullOrWhiteSpace(player.Email);
                player.IsTextEnabled = !string.IsNullOrWhiteSpace(player.TextEmail);
                
                _repository.Attach(player);
                //db.ObjectStateManager.ChangeObjectState(player, EntityState.Modified);
                _repository.SaveChanges();
                
                return RedirectToAction("Index");
            }
            return View(player);
        }

        //
        // GET: /Players/Delete/5
/* 
        public ActionResult Delete(int id)
        {
            Player player = db.Players.Single(p => p.PlayerID == id);
            return View(player);
        }

        //
        // POST: /Players/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Player player = db.Players.Single(p => p.PlayerID == id);
            db.Players.DeleteObject(player);
            db.SaveChanges();
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