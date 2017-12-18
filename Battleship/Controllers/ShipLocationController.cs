using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/ShipLocation")]
    public class ShipLocationController : BaseController
    {
        private readonly ShipLocationRepo _shipLocationRepo;
        private readonly ShotRepo _shotRepo;
        private readonly PlayerRepo _playerRepo;

        /// <summary>
        /// Controller for the ShipLocation table
        /// </summary>
        /// <param name="shipLocationRepo"></param>
        /// <param name="shotRepo"></param>
        /// <param name="playerRepo"></param>
        public ShipLocationController(ShipLocationRepo shipLocationRepo, ShotRepo shotRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _shipLocationRepo = shipLocationRepo;
            _shotRepo = shotRepo;
        }

        //
        /// <summary>
        /// Gets all of the ship locations for a board.
        /// GET: api/ShipLocation/board/{boardId}
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("board/{boardId}")]
        public JsonResult GetAllShipLocation(int boardId)
        {
            return Json(_shipLocationRepo.GetAllShipLocationsForBoard(boardId));
        }

        /// <summary>
        /// Get all of the locations for a specific ship.
        /// GET: api/ShipLocation/ship/{shipId}
        /// </summary>
        /// <param name="shipId"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("ship/{shipId}")]
        public JsonResult GetAllLocationsForShip(int shipId)
        {
            return Json(_shipLocationRepo.GetShipLocation(shipId));
        }

        /// <summary>
        /// Checks to see if a shot hit a ship location.
        /// Creates a new shot at the target location.
        /// If the shot was a hit, will check to see if all
        /// of the ship locations have been hit.
        /// GET: api/ShipLocation/checkHit
        /// </summary>
        /// <param name="shipLocation"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        [Route("checkHit")]
        public JsonResult CheckForHit(db_ShipLocation shipLocation)
        {
            // Check to see if there is a ship at the target location
            var ship = _shipLocationRepo.CheckLocation(shipLocation); // ship found at the row/col combination on the board id

            // Create the new shot
            var shot = new db_Shot
            {
                Board_Id = shipLocation.Board_Id,
                Col = shipLocation.Col,
                Row = shipLocation.Row,
                Is_Hit = (ship != null) ? 1 : 0
            };

            var newShotReturn = _shotRepo.CreateNewShot(shot);

            // Check to make sure the shot was made.
            if (newShotReturn != null)
            {
                if (!newShotReturn[0].Equals("Shot made"))
                    return Json(new
                    {
                        errMsg = "Error creating shot, please try again.",
                        err = "An error creating your shot has occured.",
                        invalidToken = false
                    });
  
                // Return the win if the repo says you won
                if (newShotReturn[1].Equals("Win"))
                {
                    return Json(new
                    {
                        shotMade = true,
                        hit = shot.Is_Hit,
                        win = true,
                        winningBoard = shot.Board_Id
                    });
                }

                // Return without the win if there is no win condition
                return Json(new
                {
                    shotMade = true,
                    win = false
                });
            }
            return Json(new
            {
                errMsg = "Error creating shot, please try again.",
                err = "An error creating your shot has occured.",
                invalidToken = false
            });
        }

        /// <summary>
        /// Create a new ship location in the DB.
        /// GET: api/ShipLocation/createLocation
        /// </summary>
        /// <param name="shipLocation"></param>
        /// <returns>bool</returns>
        [HttpPost]
        [Route("createLocation")]
        public bool CreateNewShipLocation(db_ShipLocation shipLocation)
        {
            var ship = _shipLocationRepo.CheckLocation(shipLocation);
            return ship != null || _shipLocationRepo.CreateNewShipLocation(shipLocation);
        }

        /// <summary>
        /// Update a ship location in the DB.
        /// GET: api/ShipLocation/updateLocation
        /// </summary>
        /// <param name="shipLocation"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("updateLocation")]
        public JsonResult UpdateShipLocation(db_ShipLocation shipLocation)
        {
            return Json(_shipLocationRepo.UpdateShipLocation(shipLocation));
        }
    }
}