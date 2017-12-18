using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Battleship.Cl;
using Battleship.Helpers;
using Battleship.Models;
using Battleship.Repos;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/Base")]
    public class BaseController : Controller
    {
        public readonly PlayerRepo _playerRepo; // DB repo class
 
        /// <summary>
        /// Creates the base controller used by all
        /// other controllers. Holds functions used
        /// throughout the controllers.
        /// </summary>
        /// <param name="playerRepo"></param>
        public BaseController(PlayerRepo playerRepo)
        {
            _playerRepo = playerRepo;
        }

        // ================ TOKEN METHODS ====================

        /// <summary>
        /// Validates the token provided
        /// </summary>
        /// <param name="token">Token from cookie</param>
        /// <returns></returns>
        public bool ValidateToken(string token)
        {
            if (token.Contains("%7C"))
                token = token.Replace("%7C", "|");

            var tokenChecker = new Token();
            var decodedToken = tokenChecker.DecodeToken(token);

            var ip = decodedToken.GetValue("ip").ToString();
            var requestIp = tokenChecker.RemoveChars(GetRequestIP());
            var userId = Int32.Parse(decodedToken.GetValue("id").ToString());
            var user = GetUser(userId);
            var dbIp = user.Ip;

            return ip.Equals(requestIp) && ip.Equals(dbIp);
        }

        // =================== USER METHODS ======================

        /// <summary>
        /// Retreives the user from the ID specified.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>db_Player</returns>
        public db_Player GetUser(int id)
        {
            return _playerRepo.GetPlayerById(id);
        }

        /// <summary>
        /// Retreives the user ID from the token provided.
        /// </summary>
        /// <param name="token">Token from cookie</param>
        /// <returns>int userId</returns>
        public int GetUserIdFromToken(string token)
        {
            var tokenChecker = new Token();
            var decodedToken = tokenChecker.DecodeToken(token);
            return Int32.Parse(decodedToken.GetValue("id").ToString());
        }

        /// <summary>
        /// Checks to see if a handle already exists
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>bool</returns>
        public bool HandleExists(string handle)
        {
            return _playerRepo.HandleExists(handle);
        }

        /// <summary>
        /// Gets a user from handle provided
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>db_Player</returns>
        public db_Player GetUserByHandle(string handle)
        {
            return _playerRepo.GetUserByHandle(handle);
        }

        // ================== SANITIZATION METHODS ===================

        /// <summary>
        /// Sanitizes a handle. Only allows alphanumeric characters,
        /// _, and -
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool SanitizeHandle(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(handle));

            var whiteList = new Regex("^[a-zA-Z0-9_-]+$");
            return whiteList.IsMatch(handle);
        }

        /// <summary>
        /// Sanitizes any password provided. 
        /// Will not allow characters other than alphanumeric,
        /// _, -, !, ?, @, $, or & characters
        /// </summary>
        /// <param name="password"></param>
        /// <returns>bool</returns>
        public bool SanitizePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            var whiteList = new Regex(@"^[a-zA-Z0-9_\-!?@$&]+$");
            return whiteList.IsMatch(password);
        }


        // ================ HELPER FUNCTIONS ============================

        /// <summary>
        /// Will generate a random salt for a new user
        /// </summary>
        /// <returns>string salt</returns>
        public string GenerateSalt()
        {
            var salt = "";
            var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[16];

            // Fill the bytes array
            rng.GetBytes(bytes);

            // Convert to string to store
            salt = Convert.ToBase64String(bytes);

            return salt;
        }

        /// <summary>
        /// Combines a password and it's salt, then hashes the result
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>string hashed password</returns>
        public string HashPassword(string password, string salt)
        {
            var passSalt = password + salt;
            var sha256 = SHA256.Create();

            var utf8Bytes = Encoding.UTF8.GetBytes(passSalt);
            var hashedBytes = sha256.ComputeHash(utf8Bytes);
            var hashedPassword = HexStringFromBytes(hashedBytes);

            return hashedPassword;
        }

        /// <summary>
        /// Hashes an array of bytes. Code found from: 
        /// https://gist.github.com/kristopherjohnson/3021045
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>string hashed bytes</returns>
        public string HexStringFromBytes(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the IP address of the user
        /// </summary>
        /// <param name="tryUseXForwardHeader"></param>
        /// <returns>string IP address</returns>
        public string GetRequestIP(bool tryUseXForwardHeader = true)
        {
            string ip = null;

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
            {
                ip = "X-Forwarded-For".GetHeaderValueAs<string>(HttpContext).SplitCsv().FirstOrDefault();
            }

            if (ip.IsNullOrWhitespace())
            {
                ip = Request.Headers["X-Original-For"]; 
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 
            if (ip.IsNullOrWhitespace() && HttpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            if (ip.IsNullOrWhitespace())
            {
                ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
            }

            if (ip.IsNullOrWhitespace())
            {
                ip = "REMOTE_ADDR".GetHeaderValueAs<string>(HttpContext);
            }

            if (ip.IsNullOrWhitespace())
                throw new Exception("Unable to determine caller's IP.");

            return ip;
        }
    }
}