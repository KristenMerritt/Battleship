using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Battleship.Config;
using Battleship.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace Battleship.Repos
{
    public class PlayerRepo
    {
	    private DataContext _context;

        /// <summary>
        /// Repo for the player table. Communicates directly
        /// with the database.
        /// </summary>
        /// <param name="context"></param>
	    public PlayerRepo(DataContext context)
	    {
		    _context = context;
	    }

        /// <summary>
        /// Gets all players in the database.
        /// </summary>
        /// <returns>IEnumerable<db_Player></returns>
	    public IEnumerable<db_Player> GetAllPlayers()
	    {
		    return _context.MySqlDb.Query<db_Player>("SELECT * FROM player;",
			    commandType: CommandType.Text);
	    }

        /// <summary>
        /// Adds a new player to the database.
        /// </summary>
        /// <param name="player"></param>
        public void AddNewPlayer(db_Player player)
        {
            _context.MySqlDb.Query<Player>(
                "INSERT INTO player (password, handle, salt) VALUES ('"+player.Password+"', '"+player.Handle+"', '"+player.Salt+"');",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Checks to see if a handle exists or not.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>bool</returns>
        public bool HandleExists(string handle)
        {
            var player = _context.MySqlDb.Query<db_Player>("SELECT * FROM player WHERE handle = '"+handle+"';",
                commandType: CommandType.Text).ToList();

            return player.Any();
        }

        /// <summary>
        /// Gets a user from the handle provided.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>db_Player</returns>
        public db_Player GetUserByHandle(string handle)
        {
            var player = _context.MySqlDb.Query<db_Player>(
                "SELECT * FROM player WHERE handle = '"+handle+"'",
                commandType: CommandType.Text).FirstOrDefault();

            return player;
        }

        /// <summary>
        /// Gets a player from the id provided.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>db_Player</returns>
        public db_Player GetPlayerById(int id)
        {
            try
            {
                var player = _context.MySqlDb.Query<db_Player>(
                    "SELECT * FROM player WHERE player_id = " + id + ";",
                    commandType: CommandType.Text).FirstOrDefault();

                return player;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetPlayerById");
                Debug.WriteLine(mysqlex.InnerException);
                return new db_Player();
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetPlayerById");
                Debug.WriteLine(ioe.InnerException);
                return new db_Player();
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetPlayerById");
                Debug.WriteLine(e.InnerException);
                return new db_Player();
            }
            
        }

        /// <summary>
        /// Sets the IP address of a player.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        public void SetIpAddress(string ip, int id)
        {
            _context.MySqlDb.Query<db_Player>("UPDATE player SET ip='"+ip+"' WHERE player_id="+id+";", commandType: CommandType.Text);
            Debug.WriteLine("SET IP ADDRESS");
        }
    }
}
