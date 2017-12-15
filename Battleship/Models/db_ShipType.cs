using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleship.Models
{
    public class db_ShipType
    {
        public int Ship_Type_Id { get; set; }
        public string Ship_Type { get; set; }
        public int Ship_Length { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
