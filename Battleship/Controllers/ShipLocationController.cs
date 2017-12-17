using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Http;
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

        public ShipLocationController(ShipLocationRepo shipLocationRepo, ShotRepo shotRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _shipLocationRepo = shipLocationRepo;
            _shotRepo = shotRepo;
        }

        //GET: api/ShipLocation/board/1
        [HttpGet]
        [Route("board/{boardId}")]
        public JsonResult GetAllShipLocation(int boardId)
        {
            return Json(_shipLocationRepo.GetAllShipLocationsForBoard(boardId));
        }

        //GET: api/ShipLocation/ship/1
        [HttpGet]
        [Route("ship/{shipId}")]
        public JsonResult GetAllLocationsForShip(int shipId)
        {
            return Json(_shipLocationRepo.GetShipLocation(shipId));
        }

        //GET: api/ShipLocation/row/1
        [HttpGet]
        [Route("row/{rowNum}")]
        public JsonResult GetAllShipsInRow(int rowNum)
        {
            return Json(_shipLocationRepo.GetAllShipLocationsInRow(rowNum));
        }

        //GET: api/ShipLocation/col/1
        [HttpGet]
        [Route("col/{col}")]
        public JsonResult GetAllShipsInCol(int colNum)
        {
            return Json(_shipLocationRepo.GetAllShipLocationsInCol(colNum));
        }

        //GET: api/ShipLocation/checkHit/1/1
        [HttpPost]
        [Route("checkHit")]
        public JsonResult CheckForHit(db_ShipLocation shipLocation)
        {
            var ship = _shipLocationRepo.CheckLocation(shipLocation); // ship found at the row/col combination on the board id
            var shot = new db_Shot
            {
                Board_Id = shipLocation.Board_Id,
                Col = shipLocation.Col,
                Row = shipLocation.Row,
                Is_Hit = (ship != null) ? 1 : 0
            };

            return _shotRepo.CreateNewShot(shot) ? Json(shot) : Json(new
            {
                errMsg = "Error creating shot, please try again.",
                err="An error creating your shot has occured.",
                invalidToken = false
            });
        }

        //GET: api/ShipLocation/createLocation
        [HttpPost]
        [Route("createLocation")]
        public bool CreateNewShipLocation(db_ShipLocation shipLocation)
        {
            var ship = _shipLocationRepo.CheckLocation(shipLocation);
            return ship != null || _shipLocationRepo.CreateNewShipLocation(shipLocation);
        }

        //GET: api/ShipLocation/updateLocation
        [HttpGet]
        [Route("updateLocation")]
        public JsonResult UpdateShipLocation(db_ShipLocation shipLocation)
        {
            return Json(_shipLocationRepo.UpdateShipLocation(shipLocation));
        }
    }
}