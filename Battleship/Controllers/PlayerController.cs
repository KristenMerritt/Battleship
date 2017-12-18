using System.Collections.Generic;
using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Player")]
    public class PlayerController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class

        /// <summary>
        /// Controller for the Player table
        /// </summary>
        /// <param name="playerRepo"></param>
        public PlayerController(PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
        }

        /// <summary>
        /// Gets all players from the DB
        /// GET: api/Player
        /// </summary>
        /// <returns>JsonResult</returns>
        [HttpGet]
        public JsonResult GetAllPlayers()
        {
            return Json(_playerRepo.GetAllPlayers());
;        }

        /// <summary>
        /// Gets a player by their ID.
        /// GET: api/Player/by-id/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("by-id/{id}")]
        public JsonResult GetPlayerById(int id)
        {
            return Json(base.GetUser(id));
        }

        /// <summary>
        /// Gets a player by the token provided.
        /// GET: api/Player/by-token/{token}
        /// </summary>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("by-token/{token}")]
        public JsonResult GetPlayerByToken(string token)
        {
            if (!base.ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                err = "Invalid token.",
                invalidToken = true
            });

            var id = base.GetUserIdFromToken(token);
            return Json(_playerRepo.GetPlayerById(id));
        }

        /// <summary>
        /// Gets a player based off of handle.
        /// GET: api/Player/{handle}/{token}
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="token"></param>
        /// <returns>JsonResult</returns>
        [HttpGet]
        [Route("by-handle/{handle}/{token}")]
        public JsonResult GetPlayerByHandle(string handle, string token)
        {
            if (!base.ValidateToken(token)) return Json(new
            {
                errMsg = "Invalid token detected. Please log in again.",
                err = "Invalid token.",
                invalidToken = true
            });

            return Json(base.GetUserByHandle(handle));
        }

        /// <summary>
        /// Creates a new player in the database with provided information.
        /// POST: api/Player
        /// </summary>
        /// <param name="playerInfo"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult CreatePlayer(db_Player playerInfo)
        {
            var success = true;
            var errors = new List<string>();

            if (!base.SanitizeHandle(playerInfo.Handle))
            {
                success = false;
                errors.Add("Handles can only contain letters, numbers, _ and -.");
            }

            if (!base.SanitizePassword(playerInfo.Password))
            {
                success = false;
                errors.Add("Passwords can only contain letters, numbers, and the following characters: _ - ! ? @ $ &");
            }

            // If the sanitation methods have passed, we can safely use them in our DB methods
            if (success)
            {
                if (base.HandleExists(playerInfo.Handle))
                {
                    success = false;
                    errors.Add("Handle already in use.");
                }

                if (success)
                {
                    var unhashedPassword = playerInfo.Password;
                    playerInfo.Salt = base.GenerateSalt();
                    playerInfo.Password = base.HashPassword(unhashedPassword, playerInfo.Salt);
                    _playerRepo.AddNewPlayer(playerInfo); // Adding the player to the database with a random salt and hashed password
                }
            }

            var result = new
            {
                success,
                errors
            };

            return Json(result);
        }
    }
}