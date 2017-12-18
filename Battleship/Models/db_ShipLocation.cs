using System;

namespace Battleship.Models
{
    public class db_ShipLocation
    {
        public int Ship_Location_Id { get; set; }
        public int Ship_Id { get; set; }
        public int Board_Id { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
