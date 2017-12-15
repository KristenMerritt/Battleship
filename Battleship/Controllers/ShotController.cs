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
    [Route("api/Shot")]
    public class ShotController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class
        private readonly ShotRepo _shotRepo;

        public ShotController(ShotRepo shotRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
            _shotRepo = shotRepo;
        }

        // GET: api/Shot/all-by-board/{boardId}
        // Gets all shots for a board from the DB
        // RETURN: JsonResult
        [HttpGet]
        [Route("all-by-board/{boardId}")]
        public JsonResult GetShotsForBoard(int boardId)
        {
            return Json(_shotRepo.GetAllShotsForBoard(boardId));
        }

        // GET: api/Shot/all-hits-by-board/{boardId}
        // Gets all hits for a board from the DB
        // RETURN: JsonResult
        [HttpGet]
        [Route("all-hits-by-board/{boardId}")]
        public JsonResult GetHitsForGame(int boardId)
        {
            return Json(_shotRepo.GetAllHitsForBoard(boardId));
        }

        // GET: api/Shot/all-hits-by-board/{boardId}
        // Gets all hits for a board from the DB
        // RETURN: JsonResult
        [HttpPost]
        [Route("all-hits-by-board/{boardId}")]
        public bool CreateShot(db_Shot shot)
        {
            return _shotRepo.CreateNewShot(shot);
        }
    }
}