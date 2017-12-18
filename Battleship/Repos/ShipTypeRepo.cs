using System.Collections.Generic;
using System.Data;
using System.Linq;
using Battleship.Config;
using Battleship.Models;
using Dapper;

namespace Battleship.Repos
{
    public class ShipTypeRepo
    {
        private readonly DataContext _context;

        /// <summary>
        /// Repo for the ship type table. Communicates
        /// directly with the database.
        /// </summary>
        /// <param name="context"></param>
        public ShipTypeRepo(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the data for all ship types.
        /// </summary>
        /// <returns>IEnumerable<db_ShipType></returns>
        public IEnumerable<db_ShipType> GetAllShipTypes()
        {
            return _context.MySqlDb.Query<db_ShipType>("SELECT * FROM ship_type;",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Returns the data for a specific ship type
        /// </summary>
        /// <param name="shipType"></param>
        /// <returns>db_ShipType</returns>
        public db_ShipType GetSpecificShipType(int shipType)
        {
            return _context.MySqlDb.Query<db_ShipType>("SELECT * FROM ship_type WHERE ship_type_id = " + shipType + ";",
                commandType: CommandType.Text).FirstOrDefault();
        }   
    }
}
