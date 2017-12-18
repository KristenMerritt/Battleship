using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Game")]
    public class GameController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class
        private readonly GameRepo _gameRepo;

        /// <summary>
        /// Controller for the Game table
        /// </summary>
        /// <param name="gameRepo"></param>
        /// <param name="boardRepo"></param>
        /// <param name="playerRepo"></param>
        public GameController(GameRepo gameRepo, BoardRepo boardRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
            _gameRepo = gameRepo;
        }

        /// <summary>
        /// Gets a game from the DB.
        /// GET: api/Game/{gameId}/{token}
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("{gameId}/{token}")]
        public JsonResult GetGame(int gameId, string token)
        {
            // Ensure that we have a valid token before retrieving / modifying data
            if (!ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                err = "Invalid token detected in GetGame",
                invalidToken = true
            });

            // Retrieve the game from the DB
            var game = _gameRepo.GetGame(gameId);
            if (game == null) return Json(new
            {
                errMsg = "No game found.",
                err = "No game found from GetGame",
                invalidToken = false
            });

            // Return the game
            return Json(game);
        }

        /// <summary>
        /// Gets a game from the DB by the players involved.
        /// GET: api/Game/{player1Id}/{player2Id}/{token}
        /// </summary>
        /// <param name="player1Id"></param>
        /// <param name="player2Id"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("{player1Id}/{player2Id}/{token}")]
        public JsonResult GetActiveGameByPlayers(int player1Id, int player2Id, string token)
        {
            // Ensure that we have a valid token before retrieving / modifying data
            if (!ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                err = "Invalid token detected in GetGame",
                invalidToken = true
            });

            // Retrieve the game from the DB
            var game = _gameRepo.GetActiveGameByPlayers(player1Id, player2Id);
            if (game == null) return Json(new
            {
                errMsg = "No game found.",
                err = "Null returned from _gameRepo.GetGameByPlayers",
                invalidToken = false
            });

            // Return the game
            return Json(game);
        }

        /// <summary>
        /// Retreives all active games for a player.
        /// GET: api/Game/{playerId}/{token}
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{playerId}/{token}")]
        public JsonResult GetAllActiveGamesForPlayer(int playerId, string token)
        {
            // Ensure that we have a valid token before retreiving / modifying data
            if (!ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                invalidToken = true
            });

            // Retreive all of the active games from the DB
            var games = _gameRepo.GetAllActiveGamesForPlayer(playerId);
            if (games == null) return Json(new
            {
                errMsg = "No active games found.",
                err = "No games found from GetGameByPlayerId",
                invalidToken = false
            });

            // Return the games if there are games
            return Json(games);
        }

        /// <summary>
        /// Creates a new game based off of player Ids provided.
        /// Will also create the two player boards.
        /// POST: api/Game/{player1Id}/{player2Id}/{token}
        /// </summary>
        /// <param name="player1Id"></param>
        /// <param name="player2Id"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        [Route("{player1Id}/{player2Id}/{token}")]
        public JsonResult CreateGame(int player1Id, int player2Id, string token)
        {
            // Ensure that we have a valid token before retreiving / modifying data
            if (!ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                err = "Invalid token detected in CreateGame",
                invalidToken = true
            });

            // Create a new game object to send to the DB with data provided from params
            db_Game game = new db_Game()
            {
                Player_1_Id = player1Id,
                Player_2_Id = player2Id,
                Complete = false,
                Turn = player1Id
            };

            // Create the new game in the DB without the boards
            var createdGame = _gameRepo.CreateNewGame(game); 
            if (createdGame == null )
            {
                return Json(new
                {
                    errMsg = "Error creating new game. Please try again.",
                    err = "Null returned from _gameRepo.CreateNewGame",
                    invalidToken = false
                });
            }
            return Json(createdGame); 
        }

        /// <summary>
        /// Sets the status of a game.
        /// POST: api/Game/set-game-status/{gameId}/{status}/{token}
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="status"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        [Route("set-game-status/{gameId}/{status}/{token}")]
        public JsonResult SetGameStatus(int gameId, bool status, string token)
        {
            // Ensure that we have a valid token before retreiving / modifying data
            if (!ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                invalidToken = true
            });

            // Set the game status in the DB
            var success = _gameRepo.SetGameStatus(gameId, status);

            return Json(new
            {
                gameStatusSet = success
            });
        }

        /// <summary>
        /// Starts a game over by deleteing shots and ship locations.
        /// POST: api/Game/start-over/{gameId}/{token}
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        [Route("start-over/{gameId}/{token}")]
        public JsonResult StartGameOver(int gameId, string token)
        {
            // Ensure that we have a valid token before retreiving / modifying data
            if (!ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                invalidToken = true
            });

            // Retreive all of the active games from the DB
            if (_gameRepo.RestartGame(gameId))
            {
                return Json(true);
            }

            return Json(new
            {
                errMsg = "Error restarting game. Please try again.",
                err = "Failure to restart game.",
                invalidToken = false
            });
        }
    }
}