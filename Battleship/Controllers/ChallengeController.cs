using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Challenge")]
    public class ChallengeController : BaseController
    {
        private readonly ChallengeRepo _challengeRepo;
        private readonly PlayerRepo _playerRepo;
        private readonly GameRepo _gameRepo;

        /// <summary>
        /// Controller ofr the Challenge table
        /// </summary>
        /// <param name="challengeRepo"></param>
        /// <param name="gameRepo"></param>
        /// <param name="playerRepo"></param>
        public ChallengeController(ChallengeRepo challengeRepo, GameRepo gameRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _challengeRepo = challengeRepo;
            _gameRepo = gameRepo;
        }

        /// <summary>
        /// Gets a specific challenge.
        /// GET: api/Challenge/get-challenge/{challengeId}/{token}
        /// </summary>
        /// <param name="challengeId"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("get-challenge/{challengeId}/{token}")]
        public JsonResult GetChallenge(int challengeId, string token)
        {
            if (!base.ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                invalidToken = true
            });
            return Json(_challengeRepo.GetChallenge(challengeId));
        }

        /// <summary>
        /// Gets all of the pending or accepted challenges
        /// for a particular player.
        /// GET: api/Challenge/{playerId}/{token}
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("{playerId}/{token}")]
        public JsonResult GetAllPendingOrAcceptedChallenges(int playerId, string token)
        {
            if (!base.ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                invalidToken = true
            });
            return Json(_challengeRepo.GetAllPendingOrAcceptedChallenges(playerId));
        }

        /// <summary>
        /// Gets all of the recent pending challenges for a 
        /// particular player.
        /// GET: api/Challenge/checkNew/{playerId}/{challengeId}/{token}
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="challengeId"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("checkNew/{playerId}/{challengeId}/{token}")]
        public JsonResult GetAllRecentPendingChallenge(int playerId, int challengeId, string token)
        {
            if (!base.ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                invalidToken = true
            });
            return Json(_challengeRepo.GetAllRecentPendingPlayerChallenges(playerId, challengeId));
        }

        /// <summary>
        /// Sets the status of a particular challenge. If the challenge
        /// was accepted, make a new game for the two players.
        /// POST: api/Challenge/set-status/{challengeId}/{status}/{token}
        /// </summary>
        /// <param name="challengeId"></param>
        /// <param name="status"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("set-status/{challengeId}/{status}/{token}")]
        public JsonResult SetStatusOfChallenge(int challengeId, bool status, string token)
        {
            if (!base.ValidateToken(token)) return Json( new
            {
                errMsg = "Invalid token detected - status of challenge not set.",
                invalidToken = true
            });

            // Sets the status of the challenge
            var updatedChallenge = _challengeRepo.SetStatus(status, challengeId);

            // Check to make sure we actually did update the challenge.
            if (updatedChallenge == null)
            {
                // If we did not update the challenge, send an error message
                return Json(new
                {
                    errMsg = "Challenge status not set - please try again.",
                    invalidToken = false
                });
            }

            // Check to see if the challenge was accepted, make a new game
            if (updatedChallenge.Accepted)
            {
                db_Game game = new db_Game()
                {
                    Player_1_Id = updatedChallenge.Player_1,
                    Player_2_Id = updatedChallenge.Player_2,
                    Complete = false,
                    Turn = updatedChallenge.Player_1
                };

                // Make the game
                var gameCreated = _gameRepo.CreateNewGame(game);

                // Check to make sure the game was created
                if (gameCreated == null)
                {
                    // Return an error if the game was not made
                    return Json(new
                    {
                        errMsg = "Game could not be created. You may already have an active game with this user.",
                        err = "Null returned from creating a game",
                        invalidToken = false
                    });
                } 
            }

            // Return the normal updated challenge if the game was not accepted
            return Json(updatedChallenge);
        }

        /// <summary>
        /// Add a new challenge to the DB.
        /// POST: api/Challenge/new/{player1Id}/{player2Id}/{token}
        /// </summary>
        /// <param name="player1Id"></param>
        /// <param name="player2Id"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        [Route("new/{player1Id}/{player2Id}/{token}")]
        public JsonResult AddNewChallenge(int player1Id, int player2Id, string token)
        {
            if (!base.ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected - challenge not sent.",
                err = "Invalid token.",
                invalidToken = true
            });

            var challenge = new db_Challenge
            {
                Player_1 = player1Id,
                Player_2 = player2Id
            };

            if (_challengeRepo.AddNewChallenge(challenge))
            {
                return Json(new
                {
                    challengeCreated = true
                });
            }

            return Json(new
            {
                errMsg = "Error creating challenge, please try again.",
                err = "Challenge not created.",
                invalidToken = false
            });
        }
    }
}