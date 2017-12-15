using System;
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

        public ShipTypeRepo(DataContext context)
        {
            _context = context;
        }

        // Returns the data for all ship types
        // RETURN: IEnumerable<db_ShipType>
        public IEnumerable<db_ShipType> GetAllShipTypes()
        {
            return _context.MySqlDb.Query<db_ShipType>("SELECT * FROM ship_type;",
                commandType: CommandType.Text);
        }

        // Returns the data for a specific ship type
        // PARAM: int shipType
        // RETURN: db_ShipType
        public db_ShipType GetSpecificShipType(int shipType)
        {
            return _context.MySqlDb.Query<db_ShipType>("SELECT * FROM ship_type WHERE ship_type_id = " + shipType + ";",
                commandType: CommandType.Text).FirstOrDefault();
        }   
    }
}
