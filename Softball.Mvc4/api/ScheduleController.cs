using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Softball.Mvc4.Helpers;

namespace Softball.Mvc4.api.Schedule.Controllers
{
    public class ScheduleController : ApiController
    {
        IRepository _repository;

        //
        // Dependency Injection enabled constructors

        public ScheduleController() : this(new Repository()) { }
        public ScheduleController(IRepository repository) { _repository = repository; }

        // GET /api/<controller>
        public IEnumerable<ScheduleItem> Get()
        {
            var query = _repository.FindGames()
                                  .Where(g => g.GameDate.Value.Year == DateTime.Today.Year)
                                  .OrderBy(g => g.GameDate);

            var list = new List<ScheduleItem>();
            foreach (var game in query)
            {
                var item = new ScheduleItem
                {
                    id = game.GameID,
                    title = game.Opponent + (string.IsNullOrWhiteSpace(game.Game1Result) ? "" : string.Format(" ({0}/{1})", game.Game1Result, game.Game2Result)),
                    start = game.GameDate.Value.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                list.Add(item);
            }

            return list;
        }

        // GET /api/<controller>/5
        public ScheduleItem Get(int id)
        {
            var game = _repository.FindGames()
                                  .Where(g => g.GameID == id)
                                  .Single();

            return new ScheduleItem
            {
                id = game.GameID,
                title = game.Opponent,
                start = game.GameDate.Value.ToString("MM-dd-yyyy hh:mm")
            };
        }

        // POST /api/<controller>
        public void Post(string value)
        {
        }

        // PUT /api/<controller>/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/<controller>/5
        public void Delete(int id)
        {
        }

        public class ScheduleItem
        {
            public int id { get; set; }
            public string title { get; set; }
            public string start { get; set; }
        }
    }
}