using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleship.Models
{
    public class db_Player
    {
        public int Player_Id { get; set; }
        public string Handle { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Ip { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }
    }
}
