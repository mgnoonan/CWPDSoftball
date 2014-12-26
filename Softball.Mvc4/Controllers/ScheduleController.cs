using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Softball.Mvc4.Helpers;
using System.Text;

namespace Softball.Mvc4.Controllers
{ 
    public class ScheduleController : _Controller
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public ScheduleController() : this(new Repository()) { }
        public ScheduleController(IRepository repository) { _repository = repository; }

        //
        // GET: /Game/

        public ViewResult Index()
        {
            var list = _repository.FindGames()
                                  //.Where(g => g.GameDate.Value.Year == targetDate.Year)
                                  .OrderByDescending(g => g.GameDate)
                                  .ToList();
            
            return View(list);
        }

        //
        // GET: /Game/Details/5
        /*
        public ViewResult Details(int id)
        {
            Game game = _repository.FindGames().Single(g => g.GameID == id);
            return View(game);
        }
        */
        //
        // GET: /Game/Create

        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Game/Create

        [Authorize]
        [HttpPost]
        public ActionResult Create(Game game)
        {
            if (ModelState.IsValid)
            {
                // All games start at 6:15pm
                game.GameDate = DateTime.Parse(game.GameDate.Value.ToString("MM/dd/yyyy") + " 18:15:00");

                _repository.AddObject(game);
                _repository.SaveChanges();

                return RedirectToAction("Index");  
            }

            return View(game);
        }

        //
        // GET: /Game/Edit/5

        [Authorize]
        public ActionResult Edit(int id)
        {
            Game game = _repository.FindGames().Single(g => g.GameID == id);
            return View(game);
        }

        //
        // POST: /Game/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Game game)
        {
            if (ModelState.IsValid)
            {
                var obj = _repository.FindGames().Single(g => g.GameID == game.GameID);
                obj.Game1Result = game.Game1Result;
                obj.Game2Result = game.Game2Result;

                //_repository.Attach(game);
                _repository.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        //
        // GET: /Game/Delete/5
 /*
        public ActionResult Delete(int id)
        {
            Game game = _repository.FindGames().Single(g => g.GameID == id);
            return View(game);
        }

        //
        // POST: /Game/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Game game = _repository.FindGames().Single(g => g.GameID == id);
            _repository.DeleteObject(game);
            _repository.SaveChanges();
            return RedirectToAction("Index");
        }
        */
        public FileContentResult ScheduleDownload()
        {
            //var obj = new Game();

            //obj.Query.Load();

            //var games = from g in obj.ToList()
            //            where g.GameLocation != "TBD"
            //            orderby g.GameDate ascending
            //            select g;

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:-//noonan.dns2go.com//Schedule Calendar 0.001//EN");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("X-WR-CALNAME:2009 Bargos Softball Schedule");
            sb.AppendLine("X-WR-TIMEZONE:US/Eastern");
            sb.AppendLine("BEGIN:VTIMEZONE");
            sb.AppendLine("TZID:US/Eastern");
            sb.AppendLine("X-LIC-LOCATION:America/Centerville");
            sb.AppendLine("BEGIN:DAYLIGHT");
            sb.AppendLine("TZOFFSETFROM:-0500");
            sb.AppendLine("TZOFFSETTO:-0400");
            sb.AppendLine("TZNAME:EDT");
            sb.AppendLine("DTSTART:19700308T020000");
            sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=3;BYDAY=2SU");
            sb.AppendLine("END:DAYLIGHT");
            sb.AppendLine("BEGIN:STANDARD");
            sb.AppendLine("TZOFFSETFROM:-0400");
            sb.AppendLine("TZOFFSETTO:-0500");
            sb.AppendLine("TZNAME:EST");
            sb.AppendLine("DTSTART:19701101T020000");
            sb.AppendLine("RRULE:FREQ=YEARLY;BYMONTH=11;BYDAY=1SU");
            sb.AppendLine("END:STANDARD");
            sb.AppendLine("END:VTIMEZONE");

            //foreach (GameClientProxy game in games)
            //{
            //    sb.AppendLine("BEGIN:VEVENT");
            //    sb.AppendFormat("DTSTART;TZID=US/Eastern:{0}\n", game.GameDate.Value.ToString("yyyyMMddTHHmmss"));
            //    sb.AppendFormat("DTEND;TZID=US/Eastern:{0}\n", game.GameDate.Value.AddHours(2.5).ToString("yyyyMMddTHHmmss"));
            //    //sb.AppendLine("DTSTAMP:20090506T004105Z");
            //    sb.AppendFormat("UID:{0}@noonan.dns2go.com\n", game.GameID.Value);
            //    //sb.AppendLine("CREATED:20081218T175518Z");
            //    sb.AppendFormat("DESCRIPTION:{0}\n", game.AtHomeFirst.Value ? string.Format("{0} vs. Bargos", game.Opponent) : string.Format("Bargos vs. {0}", game.Opponent));
            //    //sb.AppendLine("LAST-MODIFIED:20090226T160344Z");
            //    sb.AppendFormat("LOCATION:{0}\n", game.GameLocation);
            //    sb.AppendFormat("SUMMARY:{0}\n", game.AtHomeFirst.Value ? string.Format("{0} vs. Bargos", game.Opponent) : string.Format("Bargos vs. {0}", game.Opponent));
            //    sb.AppendLine("TRANSP:TRANSPARENT");
            //    sb.AppendLine("SEQUENCE:2");
            //    sb.AppendLine("URL:http://softball.noonan.dns2go.com");
            //    sb.AppendLine("END:VEVENT");
            //}

            sb.AppendFormat("END:VCALENDAR");

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/calendar", "bargos.ics");
        }

        public JsonResult TeamNames()
        {
            var list = _repository.FindTeams().Where(t => t.TeamID != 1).Select(t => t.TeamName).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FieldNames()
        {
            var list = _repository.FindGames()
                                  .OrderBy(t => t.GameLocation)
                                  .Select(t => t.GameLocation)
                                  .Distinct()
                                  .ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _repository.Dispose();
            base.Dispose(disposing);
        }
    }
}