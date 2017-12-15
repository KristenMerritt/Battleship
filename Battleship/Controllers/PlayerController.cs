using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const string InvalidTokenMsg = "Token not validated.";

        public PlayerController(PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
        }

        // GET: api/Player
        // Gets all players from the DB
        // RETURN: JsonResult
        [HttpGet]
        public JsonResult GetAllPlayers()
        {
            return Json(_playerRepo.GetAllPlayers());
;        }

        // GET: api/Player/1/token
        // Gets a player by their ID
        // PARAM: int id
        // PARAM: string token
        // RETURN: JsonResult
        [HttpGet]
        [Route("by-id/{id}")]
        public JsonResult GetPlayerById(int id)
        {
            return Json(base.GetUser(id));
        }

        // GET: api/Player/token
        // Gets a player by the token provided
        // PARAM: string token
        // RETURN: JsonResult
        [HttpGet]
        [Route("by-token/{token}")]
        public JsonResult GetPlayerByToken(string token)
        {
            if (base.ValidateToken(token))
            {
                var id = base.GetUserIdFromToken(token);
                return Json(_playerRepo.GetPlayerById(id));
            }
            var error = new {err = InvalidTokenMsg };
            return Json(error);
        }

        // GET: api/Player/handle/token
        // Gets a player based off of handle
        // PARAM: string handle
        // PARAM: string token
        // RETURN: JsonResult
        [HttpGet]
        [Route("by-handle/{handle}/{token}")]
        public JsonResult GetPlayerByHandle(string handle, string token)
        {
            if (base.ValidateToken(token))
            {
                return Json(base.GetUserByHandle(handle));
            }
            var error = new { err = InvalidTokenMsg };
            return Json(error);
        }

        // POST: api/Player
        // Creates a new player in the database with provided information
        // PARAM: db_player playerInfo
        // RETURN: JsonResult
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