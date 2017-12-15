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

	    public PlayerRepo(DataContext context)
	    {
		    _context = context;
	    }

	    public IEnumerable<db_Player> GetAllPlayers()
	    {
		    return _context.MySqlDb.Query<db_Player>("SELECT * FROM player;",
			    commandType: CommandType.Text);
	    }

        public void AddNewPlayer(db_Player player)
        {
            _context.MySqlDb.Query<Player>(
                "INSERT INTO player (password, handle, salt) VALUES ('"+player.Password+"', '"+player.Handle+"', '"+player.Salt+"');",
                commandType: CommandType.Text);
        }

        public bool HandleExists(string handle)
        {
            var player = _context.MySqlDb.Query<db_Player>("SELECT * FROM player WHERE handle = '"+handle+"';",
                commandType: CommandType.Text).ToList();

            return player.Any();
        }

        public db_Player GetUserByHandle(string handle)
        {
            var player = _context.MySqlDb.Query<db_Player>(
                "SELECT * FROM player WHERE handle = '"+handle+"'",
                commandType: CommandType.Text).FirstOrDefault();

            return player;
        }

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

        public void SetIpAddress(string ip, int id)
        {
            _context.MySqlDb.Query<db_Player>("UPDATE player SET ip='"+ip+"' WHERE player_id="+id+";", commandType: CommandType.Text);
            Debug.WriteLine("SET IP ADDRESS");
        }
    }
}
