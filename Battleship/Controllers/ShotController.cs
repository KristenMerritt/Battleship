using Battleship.Repos;
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

        // GET: api/Shot/new-shot/{shotId}/{boardId}
        // Gets all shots greater than the shot ID for a 
        // specific board.
        // RETURN: JsonResult
        [HttpGet]
        [Route("new-shots/{shotId}/{board1Id}/{board2Id}")]
        public JsonResult GetNewShotsForBoard(int shotId, int board1Id, int board2Id)
        {
            return Json(_shotRepo.GetNewShotsForBoard(shotId, board1Id, board2Id));
        }
    }
}