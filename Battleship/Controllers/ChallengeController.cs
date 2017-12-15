using System.Diagnostics;
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

        public ChallengeController(ChallengeRepo challengeRepo, GameRepo gameRepo, PlayerRepo playerRepo) : base(playerRepo)
        {
            _challengeRepo = challengeRepo;
            _gameRepo = gameRepo;
        }

        //GET: api/Challenge/5
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

        //GET: api/Challenge/5
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

        //GET: api/Challenge/getNew/{playerId}/{challengeId}
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

        //POST: api/Challenge/1/status
        [HttpPost]
        [Route("set-status/{challengeId}/{status}/{token}")]
        public JsonResult SetStatusOfChallenge(int challengeId, bool status, string token)
        {
            if (!base.ValidateToken(token)) return Json( new
            {
                errMsg = "Invalid token detected - status of challenge not set.",
                invalidToken = true
            });

            var updatedChallenge = _challengeRepo.SetStatus(status, challengeId);

            if (updatedChallenge == null)
            {
                return Json(new
                {
                    errMsg = "Challenge status not set - please try again.",
                    invalidToken = false
                });
            }

            if (updatedChallenge.Accepted)
            {
                db_Game game = new db_Game()
                {
                    Player_1_Id = updatedChallenge.Player_1,
                    Player_2_Id = updatedChallenge.Player_2,
                    Complete = false,
                    Turn = -1
                };

                var gameCreated = _gameRepo.CreateNewGame(game);

                if (gameCreated == null)
                {
                    return Json(new
                    {
                        errMsg = "Game could not be created. You may already have an active game with this user.",
                        err = "Null returned from creating a game",
                        invalidToken = false
                    });
                } 
            }

            return Json(updatedChallenge);
        }

        //POST: api/Challenge/1/2/token
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