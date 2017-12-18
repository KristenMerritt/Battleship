using System;
using System.Collections.Generic;
using Battleship.Cl;
using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/login")]
    public class LoginController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class

        /// <summary>
        /// Controller for the Login table
        /// </summary>
        /// <param name="playerRepo"></param>
        public LoginController(PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
        }

        /// <summary>
        /// Checks whether or not the user/pass combination is correct.
        /// If successful, a token will be provided back to the user to use.
        /// POST: api/Login
        /// </summary>
        /// <param name="login"></param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult Login(PlayerLoginInfo login)
        {
            var success = true;
            var errors = new List<string>();
            var userId = -1;
            var handle = "";
            var token = "";

            // Sanitize the handle and password before doing anything
            if (!base.SanitizeHandle(login.Handle))
            {
                success = false;
                errors.Add("Handles can only contain letters, numbers, _ and -.");
            }

            if (!base.SanitizePassword(login.Password))
            {
                success = false;
                errors.Add("Passwords can only contain letters, numbers, and the following characters: _ - ! ? @ $ &");
            }

            // If the handle/password are clean, then we can use them in our DB methods
            if (success)
            {
                if (base.HandleExists(login.Handle)) // If the handle does not exist, there is no login for it
                {
                    var playerInfo = base.GetUserByHandle(login.Handle); // DB call for the player information
                    var dbPass = playerInfo.Password;
                    var dbSalt = playerInfo.Salt;

                    // Salt and hash the password the user provided in the login form
                    var hashedPassword = base.HashPassword(login.Password, dbSalt);

                    // Check the hashed password against the one in the db
                    if (hashedPassword == dbPass)
                    {

                        userId = playerInfo.Player_Id;
                        handle = playerInfo.Handle;

                        var userIpAddressString = base.GetRequestIP();

                        if (String.IsNullOrEmpty(userIpAddressString) || userIpAddressString.Equals(""))
                        {
                            success = false;
                            errors.Add("Null or empty IP address.");
                        }
                        else
                        {
                            var tokenGenerator = new Token(userId, userIpAddressString);
                            token = tokenGenerator.GenerateToken();

                            _playerRepo.SetIpAddress(tokenGenerator.RemoveChars(userIpAddressString), userId); // Set the IP address for the user in the DB to check the token later
                        }                       
                    }
                    else
                    {
                        success = false;
                        errors.Add("Invalid login attempt.");
                    }
                }
                else
                {
                    success = false;
                    errors.Add("Invalid login attempt");
                }
            }

            var result = new
            {
                success,
                errors,
                userId,
                handle,
                token
            };

            return Json(result);
        }  
    }
}
