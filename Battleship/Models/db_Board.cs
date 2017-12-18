using System;

namespace Battleship.Models
{
    public class db_Board
    {
        public int Board_Id { get; set; }
        public int Game_Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
