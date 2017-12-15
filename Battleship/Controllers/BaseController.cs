using System;
using System.Collections.Generic;
using System.Diagnostics;
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
 
        public BaseController(PlayerRepo playerRepo)
        {
            _playerRepo = playerRepo;
        }

        // ================ TOKEN METHODS ====================

        // Validates the token provided.
        // PARAM: string token
        // RETURN: bool
        public bool ValidateToken(string token)
        {
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (ValidateToken) Validating token: " + token);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (ValidateToken) Validating token: " + token);

            if (token.Contains("%7C"))
                token = token.Replace("%7C", "|");

            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (ValidateToken) New token to validate: " + token);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (ValidateToken) New token to validate: " + token);

            var tokenChecker = new Token();
            var decodedToken = tokenChecker.DecodeToken(token);
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (ValidateToken) Decoded token json: " + decodedToken);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (ValidateToken) Decoded token json: " + decodedToken);

            var ip = decodedToken.GetValue("ip").ToString();
            var requestIp = tokenChecker.RemoveChars(GetRequestIP());
            var userId = Int32.Parse(decodedToken.GetValue("id").ToString());
            var user = GetUser(userId);
            var dbIp = user.Ip;

            return ip.Equals(requestIp) && ip.Equals(dbIp);
        }

        // =================== USER METHODS ======================

        // Retreives the user from the ID specified.
        // PARAM: int id
        // RETURN: db_Player
        public db_Player GetUser(int id)
        {
            return _playerRepo.GetPlayerById(id);
        }

        // Retreives the user ID from the token provided.
        // PARAM: string token
        // RETURN: bool
        public int GetUserIdFromToken(string token)
        {
            var tokenChecker = new Token();
            var decodedToken = tokenChecker.DecodeToken(token);
            return Int32.Parse(decodedToken.GetValue("id").ToString());
        }

        public bool HandleExists(string handle)
        {
            return _playerRepo.HandleExists(handle);
        }

        public db_Player GetUserByHandle(string handle)
        {
            return _playerRepo.GetUserByHandle(handle);
        }

        // ================== SANITIZATION METHODS ===================

        // Sanitizes a handle. Only allows alphanumeric characters,
        // _, and -
        // PARAM: string
        // RETURN: bool
        public bool SanitizeHandle(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(handle));

            var whiteList = new Regex("^[a-zA-Z0-9_-]+$");
            return whiteList.IsMatch(handle);
        }

        // Sanitizes any password provided
        // Will not allow characters other than alphanumeric,
        // _, -, !, ?, @, $, or & characters
        // PARAM: string
        // RETURN: bool
        public bool SanitizePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

            var whiteList = new Regex(@"^[a-zA-Z0-9_\-!?@$&]+$");
            return whiteList.IsMatch(password);
        }


        // ================ HELPER FUNCTIONS ============================

        // Will generate a random salt for a new user
        // RETURN: string
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

        // Combines a password and it's salt, then hashes the result
        // PARAM: string, string
        // RETURN: string
        public string HashPassword(string password, string salt)
        {
            var passSalt = password + salt;
            var sha256 = SHA256.Create();

            var utf8Bytes = Encoding.UTF8.GetBytes(passSalt);
            var hashedBytes = sha256.ComputeHash(utf8Bytes);
            var hashedPassword = HexStringFromBytes(hashedBytes);

            return hashedPassword;
        }

        // Hashes an array of bytes. Code found from: 
        // https://gist.github.com/kristopherjohnson/3021045
        // PARAM: Ienumerable<byte>
        // RETURN: string
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
                //Console.WriteLine(DateTime.Now.ToString("G") + " -- X-Forwarded-For: " + ip);
                //Debug.WriteLine(DateTime.Now.ToString("G") + " -- X-Forwarded-For: " + ip);
            }

            if (ip.IsNullOrWhitespace())
            {
                ip = Request.Headers["X-Original-For"]; // iis?
                //Console.WriteLine(DateTime.Now.ToString("G") + " -- X-Original-For: " + ip);
                //Debug.WriteLine(DateTime.Now.ToString("G") + " -- X-Original-For: " + ip);
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (ip.IsNullOrWhitespace() && HttpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
                //Console.WriteLine(DateTime.Now.ToString("G") + " -- RemoteIpAddress: " + ip);
                //Debug.WriteLine(DateTime.Now.ToString("G") + " -- RemoteIpAddress: " + ip);
            }


            if (ip.IsNullOrWhitespace())
            {
                ip = HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();
                //Console.WriteLine(DateTime.Now.ToString("G") + " -- RemoteIpAddress 2: " + ip);
                //Debug.WriteLine(DateTime.Now.ToString("G") + " -- RemoteIpAddress 2: " + ip);
            }


            if (ip.IsNullOrWhitespace())
            {
                ip = "REMOTE_ADDR".GetHeaderValueAs<string>(HttpContext);
                //Console.WriteLine(DateTime.Now.ToString("G") + " -- REMOTE_ADDR: " + ip);
                //Debug.WriteLine(DateTime.Now.ToString("G") + " -- REMOTE_ADDR: " + ip);
            }

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            if (ip.IsNullOrWhitespace())
                throw new Exception("Unable to determine caller's IP.");

            return ip;
        }
    }
}