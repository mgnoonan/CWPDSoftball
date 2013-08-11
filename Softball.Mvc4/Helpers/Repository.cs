using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Softball.Mvc4.Helpers
{
    public class Repository : IRepository
    {
        private SoftballEntities context = new SoftballEntities();

        public IQueryable<Game> FindGames()
        {
            var query = from g in context.Games 
                        where g.GameLocation.StartsWith("Yankee")
                        select g;

            return query;
        }

        public IQueryable<Game> FindNextGames(DateTime targetDate)
        {
            var query = FindGames().Where(g => g.GameDate > targetDate && g.GameLocation.StartsWith("Yankee")).OrderBy(g => g.GameDate).Take(1);

            return query;
        }

        public IQueryable<Game> FindLastGames(DateTime targetDate)
        {
            var query = FindGames().Where(g => g.GameDate <= targetDate && g.GameLocation.StartsWith("Yankee")).OrderByDescending(g => g.GameDate).Take(1);

            return query;
        }

        public IQueryable<WonLostDetailView> FindWonLostGames()
        {
            var query = from g in context.WonLostDetailViews
                        select g;

            return query;
        }

        public IQueryable<Team> FindTeams()
        {
            var query = from g in context.Teams
                        select g;

            return query;
        }

        public string GetTeamName()
        {
            var obj = FindTeams().Single(t => t.TeamID == 1);

            return obj.TeamName.Trim();
        }

        public IQueryable<Player> FindPlayers()
        {
            var query = from g in context.Players
                        select g;

            return query;
        }

        public void DeleteObject(Game obj)
        {
            context.Games.DeleteObject(obj);
        }

        public void AddObject(Game obj)
        {
            context.Games.AddObject(obj);
        }

        public void Attach(Game obj)
        {
            context.Games.Attach(obj);
            context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
        }

        public void Attach(Team obj)
        {
            context.Teams.Attach(obj);
            context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
        }

        public void DeleteObject(Team obj)
        {
            context.Teams.DeleteObject(obj);
        }

        public void AddObject(Team obj)
        {
            context.Teams.AddObject(obj);
        }

        public void Attach(Player obj)
        {
            context.Players.Attach(obj);
            context.ObjectStateManager.ChangeObjectState(obj, EntityState.Modified);
        }

        public void DeleteObject(Player obj)
        {
            context.Players.DeleteObject(obj);
        }

        public void AddObject(Player obj)
        {
            context.Players.AddObject(obj);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}