using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleship.Models
{
    public class db_Ship
    {
        public int Ship_Id { get; set; }
        public int Board_Id { get; set; }
        public int Ship_Type_Id { get; set; }
        public int Is_Placed { get; set; } 
        public int Is_Sunk { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
