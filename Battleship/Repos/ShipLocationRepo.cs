using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        // Returns all of the data points for all ships in a specific board
        // PARAM: int boardId
        // RETURN: IEnumerable<db_ShipLocation>
        public IEnumerable<db_ShipLocation> GetAllShipLocationsForBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE board_id = " + boardId + ";",
                commandType: CommandType.Text);
        }

        // Returns all of the location data points for a specific ship
        // PARAM: int shipId
        // RETURN: IEnumerable<db_ShipLocation>
        public IEnumerable<db_ShipLocation> GetShipLocation(int shipId)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE ship_id = " + shipId + ";",
                commandType: CommandType.Text);
        }

        // Returns true if hit, false if miss
        // PARAM: int row
        // PARAM: int col
        // RETURN: boolean
        public db_ShipLocation CheckLocation(db_ShipLocation shipLocation)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE board_id = " + shipLocation.Board_Id + " AND row = " + shipLocation.Row + " AND col = " + shipLocation.Col + ";",
                commandType: CommandType.Text).FirstOrDefault();
        }

        // Returns all of the data points for all ships in a specific row
        // PARAM: int row
        // RETURN: IEnumerable<db_ShipLocation>
        public IEnumerable<db_ShipLocation> GetAllShipLocationsInRow(int row)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE row = " + row + ";",
                commandType: CommandType.Text);
        }

        // Returns all of the data points for all ships in a specific col
        // PARAM: int col
        // RETURN: IEnumerable<db_ShipLocation>
        public IEnumerable<db_ShipLocation> GetAllShipLocationsInCol(int col)
        {
            return _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE col = " + col + ";",
                commandType: CommandType.Text);
        }

        // Inserts a data point for a ship
        // PARAM: db_ShipLocation
        // RETURN: bool
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

        // Update a data point for a ship
        // PARAM: db_ShipLocation
        // RETURN: bool
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
