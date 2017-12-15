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
        private readonly BoardRepo _boardRepo;

        public GameController(GameRepo gameRepo, BoardRepo boardRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
            _gameRepo = gameRepo;
            _boardRepo = boardRepo;
        }

        // GET: api/Game/{gameId}/{token}
        // Gets a game from the DB
        // RETURN: JsonResult
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

        // GET: api/Game/{player1Id}/{player2Id}
        // Gets a game from the DB
        // RETURN: JsonResult
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

        // GET: api/Game/{playerId}/{token}
        // Retreives all active games for a player
        // PARAM: int playerId
        // PARAM: string token
        // RETURN: JsonResult
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

        // POST: api/Game/{player1Id}/{player2Id}/{token}
        // Creates a new game based off of player Ids provided
        // Will also create the two player boards
        // PARAM: int player1Id
        // PARAM: int player2Id
        // PARAM: string token
        // RETURN: JsonResult
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
                Turn = -1
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

            // Create two boards for the game
            //var player1Board = _boardRepo.CreateBoard(createdGame.Game_Id); 
            //var player2Board = _boardRepo.CreateBoard(createdGame.Game_Id);
            //if (player1Board == null || player2Board == null)
            //{
            //    return Json(new
            //    {
            //        errMsg = "Error creating boards for game. Please try again.",
            //        err = "Null returned from _board.CreateBoard",
            //        invalidToken = false
            //    });
            //}

            //// Set the boards in the game object reference
            //createdGame.Player_1_Board_Id = player1Board.Board_Id; 
            //createdGame.Player_2_Board_Id = player2Board.Board_Id;

            // Set the game boards in the DB
            return Json(createdGame); 
        }

        // POST: api/Game/set-game-status/{gameId}/{status}/{token}
        // Sets the status of a game
        // PARAM: int gameId
        // PARAM: bool status
        // PARAM: string token
        // RETURN: JsonResult
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
    }
}