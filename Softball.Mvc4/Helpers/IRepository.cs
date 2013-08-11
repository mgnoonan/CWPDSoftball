using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Softball.Mvc4.Helpers
{
    public interface IRepository
    {
        IQueryable<Game> FindGames();
        IQueryable<Game> FindNextGames(DateTime targetDate);
        IQueryable<Game> FindLastGames(DateTime targetDate);
        IQueryable<WonLostDetailView> FindWonLostGames();
        IQueryable<Team> FindTeams();
        IQueryable<Player> FindPlayers();
        string GetTeamName();

        void DeleteObject(Game obj);
        void AddObject(Game obj);
        void Attach(Game obj);

        void DeleteObject(Team obj);
        void AddObject(Team obj);
        void Attach(Team obj);

        void DeleteObject(Player obj);
        void AddObject(Player obj);
        void Attach(Player obj);

        void Dispose();
        void SaveChanges();
    }
}
