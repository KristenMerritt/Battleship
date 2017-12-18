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
    public class ShipRepo
    {
        private readonly DataContext _context;

        /// <summary>
        /// repo for the Ship table. Communicates directly
        /// with the database.
        /// </summary>
        /// <param name="context"></param>
        public ShipRepo(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retreives all of the ships for a certain board.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns>IEnumerable<db_Ship></returns>
        public IEnumerable<db_Ship> GetAllShipsForBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_Ship>("SELECT * FROM ship WHERE board_id = " + boardId + ";",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Gets a specific ship from the db.
        /// </summary>
        /// <param name="shipId"></param>
        /// <returns>db_Ship</returns>
        public db_Ship GetShip(int shipId)
        {
            return _context.MySqlDb.Query<db_Ship>("SELECT * FROM ship WHERE ship_id = " + shipId + ";",
                commandType: CommandType.Text).FirstOrDefault();
        }

        /// <summary>
        /// Creates a new ship in the DB.
        /// </summary>
        /// <param name="ship"></param>
        /// <returns>bool</returns>
        public bool CreateShip(db_Ship ship)
        {
            try
            {
                _context.MySqlDb.Query<db_Ship>("INSERT INTO ship (board_id, ship_type_id, is_placed) VALUES (" + ship.Board_Id + ", " + ship.Ship_Type_Id+ ", " + ship.Is_Placed+ ");",
                    commandType: CommandType.Text);
                return true;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN CreateShip");
                Debug.WriteLine(mysqlex.InnerException);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN CreateShip");
                Debug.WriteLine(ioe.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN CreateShip");
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }

        /// <summary>
        /// Sets the placement status of a specific ship in the database
        /// </summary>
        /// <param name="shipId"></param>
        /// <param name="status"></param>
        /// <returns>bool</returns>
        public bool SetPlacementStatus(int shipId, bool status)
        {
            try
            {
                _context.MySqlDb.Query<db_Ship>("UPDATE ship SET is_placed = " + status + " WHERE ship_id = " + shipId + ";",
                    commandType: CommandType.Text);
                return true;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN SetPlacementStatus");
                Debug.WriteLine(mysqlex.InnerException);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN SetPlacementStatus");
                Debug.WriteLine(ioe.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN SetPlacementStatus");
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }
    }
}
