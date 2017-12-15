using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleship.Models
{
    public class db_Chat
    {
        public int Chat_Id { get; set; }
        public int Player_Id { get; set; }
        public string Message { get; set; }
    }
}
