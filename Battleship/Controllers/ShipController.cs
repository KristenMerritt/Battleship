using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Ship")]
    public class ShipController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class
        private readonly ShipRepo _shipRepo;

        /// <summary>
        /// Controller for the Ship table
        /// </summary>
        /// <param name="shipRepo"></param>
        /// <param name="playerRepo"></param>
        public ShipController(ShipRepo shipRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
            _shipRepo = shipRepo;
        }

        /// <summary>
        /// Gets a specific ship.
        /// GET: api/Ship/{shipId}
        /// </summary>
        /// <param name="shipId"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("ship/{shipId}")]
        public JsonResult GetShip(int shipId)
        {
            return Json(_shipRepo.GetShip(shipId));
        }

        /// <summary>
        /// Gets all of the ships for a specific board.
        /// GET: api/Ship/all-by-board/{boardId}
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("all-by-board/{boardId}")]
        public JsonResult GetShipsForBoard(int boardId)
        {
            return Json(_shipRepo.GetAllShipsForBoard(boardId));
        }

        /// <summary>
        /// Makes new ships for a new game board.
        /// POST: api/Ship/starter/{boardId}
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns>bool</returns>
        [HttpPost]
        [Route("starter/{boardId}")]
        public bool MakeStartingShips(int boardId)
        {
            var carrier = new db_Ship()
            {
                Board_Id = boardId,
                Ship_Type_Id = 1,
                Is_Placed = 0,
                Is_Sunk = 0
            };

            var battleship = new db_Ship()
            {
                Board_Id = boardId,
                Ship_Type_Id = 2,
                Is_Placed = 0,
                Is_Sunk = 0
            };

            var submarine = new db_Ship()
            {
                Board_Id = boardId,
                Ship_Type_Id = 3,
                Is_Placed = 0,
                Is_Sunk = 0
            };

            var cruiser = new db_Ship()
            {
                Board_Id = boardId,
                Ship_Type_Id = 4,
                Is_Placed = 0,
                Is_Sunk = 0
            };

            var destroyer = new db_Ship()
            {
                Board_Id = boardId,
                Ship_Type_Id = 5,
                Is_Placed = 0,
                Is_Sunk = 0
            };

            return _shipRepo.CreateShip(carrier) &&
                   _shipRepo.CreateShip(battleship) &&
                   _shipRepo.CreateShip(submarine) &&
                   _shipRepo.CreateShip(cruiser) &&
                   _shipRepo.CreateShip(destroyer);
        }
    }
}