using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/ShipType")]
    public class ShipTypeController : Controller
    {
        private readonly ShipTypeRepo _shipTypeRepo;

        /// <summary>
        /// Controller for the ShipType table.
        /// </summary>
        /// <param name="shipTypeRepo"></param>
        public ShipTypeController(ShipTypeRepo shipTypeRepo)
        {
            _shipTypeRepo = shipTypeRepo;
        }

        /// <summary>
        /// Gets a specific ship type from the DB.
        /// </summary>
        /// <param name="shipTypeId"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("{shipTypeId}")]
        public JsonResult GetShipType(int shipTypeId)
        {
            return Json(_shipTypeRepo.GetSpecificShipType(shipTypeId));
        }
    }
}