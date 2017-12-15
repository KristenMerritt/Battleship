using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/ShipType")]
    public class ShipTypeController : Controller
    {
        private readonly ShipTypeRepo _shipTypeRepo;

        public ShipTypeController(ShipTypeRepo shipTypeRepo)
        {
            _shipTypeRepo = shipTypeRepo;
        }

        // GET: api/ShipType/{shipTypeId}
        // Gets a ship type from the DB
        // RETURN: JsonResult
        [HttpGet]
        [Route("{shipTypeId}")]
        public JsonResult GetShipType(int shipTypeId)
        {
            return Json(_shipTypeRepo.GetSpecificShipType(shipTypeId));
        }
    }
}