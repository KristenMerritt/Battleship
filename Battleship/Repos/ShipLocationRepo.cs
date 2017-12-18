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
    public class ShipLocationRepo
    {
        private readonly DataContext _context;

        public ShipLocationRepo(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all of the data points for all ships in a specific board.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns>IEnumerable<db_ShipLocation></returns>
        public IEnumerable<db_ShipLocation> GetAllShipLocationsForBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE board_id = " + boardId + ";",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Returns all of the location data points for a specific ship
        /// </summary>
        /// <param name="shipId"></param>
        /// <returns>IEnumerable<db_ShipLocation></returns>
        public IEnumerable<db_ShipLocation> GetShipLocation(int shipId)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE ship_id = " + shipId + ";",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Checks a location to see if a ship is there or not.
        /// </summary>
        /// <param name="shipLocation"></param>
        /// <returns>db_ShipLocation</returns>
        public db_ShipLocation CheckLocation(db_ShipLocation shipLocation)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE board_id = " + shipLocation.Board_Id + " AND row = " + shipLocation.Row + " AND col = " + shipLocation.Col + ";",
                commandType: CommandType.Text).FirstOrDefault();
        }

        /// <summary>
        /// Inserts a data point for a ship.
        /// </summary>
        /// <param name="shipLocation"></param>
        /// <returns>bool</returns>
        public bool CreateNewShipLocation(db_ShipLocation shipLocation)
        {
            try
            {
                _context.MySqlDb.Query<db_ShipLocation>("INSERT INTO ship_location (ship_id, board_id, row, col) VALUES (" + shipLocation.Ship_Id + ", " + shipLocation.Board_Id + ", " + shipLocation.Row + ", " + shipLocation.Col + ");",
                    commandType: CommandType.Text);
                return true;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN CreateNewShipLocation");
                Debug.WriteLine(mysqlex.InnerException);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN CreateNewShipLocation");
                Debug.WriteLine(ioe.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN CreateNewShipLocation");
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }

        /// <summary>
        /// Updates a data point for a ship
        /// </summary>
        /// <param name="shipLocation"></param>
        /// <returns>bool</returns>
        public bool UpdateShipLocation(db_ShipLocation shipLocation)
        {
            try
            {
                _context.MySqlDb.Query<db_ShipLocation>("UPDATE ship_location SET row = " + shipLocation.Row + ", col = " + shipLocation.Col + " WHERE ship_id = " + shipLocation.Ship_Id + ";",
                    commandType: CommandType.Text);
                return true;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN CreateNewShipLocation");
                Debug.WriteLine(mysqlex.InnerException);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN CreateNewShipLocation");
                Debug.WriteLine(ioe.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN CreateNewShipLocation");
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }
    }
}
