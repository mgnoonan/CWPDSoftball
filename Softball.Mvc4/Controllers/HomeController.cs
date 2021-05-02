using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using HtmlAgilityPack;
using Softball.Mvc4.Helpers;
using Softball.Mvc4.Models;
using Softball.Mvc4.Utilities;

namespace Softball.Mvc4.Controllers
{
    public class HomeController : _Controller
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public HomeController() : this(new Repository()) { }
        public HomeController(IRepository repository) { _repository = repository; }

        public ActionResult Index()
        {
            var nextGame = _repository.FindNextGames(targetDate).SingleOrDefault();
            var lastGame = _repository.FindLastGames(targetDate).SingleOrDefault();
            var results = _repository.FindWonLostGames().Where(r => r.Year.Value == targetDate.Year).ToList();
            int won = 0;
            int lost = 0;
            foreach (var r in results)
            {
                won += (r.Win1 + r.Win2);
                lost += (r.Loss1 + r.Loss2);
            }

            ViewBag.TeamName = _repository.GetTeamName();
            ViewBag.Record = string.Format("{0}-{1}", won, lost);
            ViewBag.LastGame = lastGame;

            return View(nextGame);
        }

        public JsonResult Standings()
        {
            string response = Util.GetUrl("http://www.cwpd.org/adultsoftballleague.html");
            var list = GetStandingsFromDom(response);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        private List<Standing> GetStandingsFromDom(string html)
        {
            var list = new List<Standing>();
            var doc = new HtmlDocument();
            doc.Load(new StringReader(html));

            var node = doc.DocumentNode.SelectSingleNode("//table[@class='just']");
            if (node == null)
                node = doc.DocumentNode.SelectSingleNode("//table[@class='table_stripe_white']");

            var rows = node.SelectNodes("tr");
            int rowNumber = 1;
            int placeNumber = 1;

            foreach (var row in rows)
            {
                var cols = row.SelectNodes("td");
                if (cols.Count != 3)
                    continue;

                string teamName = cols[0].InnerText.Trim();
                if (string.IsNullOrWhiteSpace(teamName) || rowNumber++ == 1)
                    continue;

                list.Add(new Standing
                {
                    Place = placeNumber++,
                    TeamName = teamName.Trim(),
                    Wins = GetWinLossColumnValue(cols[1]),
                    Losses = GetWinLossColumnValue(cols[2])
                });
            }

            return list;
        }

        private int GetWinLossColumnValue(HtmlNode col)
        {
            if (string.IsNullOrWhiteSpace(col.InnerText) || col.InnerText == "&nbsp;")
                return 0;

            return Convert.ToInt32(col.InnerText);
        }

    }
}
