using System;

namespace Battleship.Models
{
    public class db_Shot
    {
        public int Shot_Id { get; set; }
        public int Board_Id { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int Is_Hit { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
