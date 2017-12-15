using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Battleship.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Board")]
    public class BoardController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class
        private readonly BoardRepo _boardRepo;

        public BoardController(BoardRepo boardRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
            _boardRepo = boardRepo;
        }

        // GET: api/Game/{gameId}/{token}
        // Gets a game from the DB
        // RETURN: JsonResult
        [HttpGet]
        [Route("{boardId}/{token}")]
        public JsonResult GetBoard(int boardId, string token)
        {
            // Ensure that we have a valid token before retrieving / modifying data
            if (!ValidateToken(token))
                return Json(new
                {
                    errMsg = "Invalid token detected. Please log in again.",
                    err = "Invalid token detected in GetGame",
                    invalidToken = true
                });

            return Json(_boardRepo.GetBoard(boardId));
        }
    }
}