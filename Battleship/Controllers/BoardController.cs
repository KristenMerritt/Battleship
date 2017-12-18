using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Board")]
    public class BoardController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class
        private readonly BoardRepo _boardRepo;

        /// <summary>
        /// Controller for Board table
        /// </summary>
        /// <param name="boardRepo"></param>
        /// <param name="playerRepo"></param>
        public BoardController(BoardRepo boardRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
            _boardRepo = boardRepo;
        }

        /// <summary>
        /// Gets a specific board from the DB.
        /// GET: api/Game/{gameId}/{token}
        /// </summary>
        /// <param name="boardId"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
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