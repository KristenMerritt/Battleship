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

        /// <summary>
        /// Controller for the Shot table.
        /// </summary>
        /// <param name="shotRepo"></param>
        /// <param name="playerRepo"></param>
        public ShotController(ShotRepo shotRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
            _shotRepo = shotRepo;
        }

        /// <summary>
        /// Gets all shots for a board from the DB.
        /// GET: api/Shot/all-by-board/{boardId}
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("all-by-board/{boardId}")]
        public JsonResult GetShotsForBoard(int boardId)
        {
            return Json(_shotRepo.GetAllShotsForBoard(boardId));
        }

        /// <summary>
        /// Gets all shots greater than the shot ID for a
        /// specific board.
        /// GET: api/Shot/new-shots/{shotId}/{board1Id}/{board2Id}
        /// </summary>
        /// <param name="shotId"></param>
        /// <param name="board1Id"></param>
        /// <param name="board2Id"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("new-shots/{shotId}/{board1Id}/{board2Id}")]
        public JsonResult GetNewShotsForBoard(int shotId, int board1Id, int board2Id)
        {
            return Json(_shotRepo.GetNewShotsForBoard(shotId, board1Id, board2Id));
        }
    }
}