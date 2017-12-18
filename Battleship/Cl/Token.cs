using System;
using Newtonsoft.Json.Linq;

namespace Battleship.Cl
{
    public class Token
    {
        private byte[] Time { get; set; }
        private int UserId { get; set; }
        private string UserIpAddress { get; set; }
        public string UserToken { get; set; }

        /// <summary>
        /// Creates, validates, decodes tokens
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ip"></param>
        public Token(int userId, string ip)
        {
            UserId = userId;
            UserIpAddress = ip;
            Time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        }

        /// <summary>
        /// Token constructor. Sets a new date.
        /// </summary>
        public Token()
        {
            Time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        }

        /// <summary>
        /// Generates a new token based off 
        /// of the user IP and ID
        /// </summary>
        /// <returns>New user token</returns>
        public string GenerateToken()
        {
            var removedChars = RemoveChars(UserIpAddress);
            var hexIp = StringToHex(removedChars); // obscured ip address changed to HEX
            var obscuredIp = ObscureToken(hexIp); // ip address with random numbers in between
            var hexId = StringToHex(UserId.ToString()); // normal ID changed to HEX

            UserToken = obscuredIp + "|" + hexId;
            return UserToken;           
        }

        /// <summary>
        /// Obscures a token
        /// </summary>
        /// <param name="str">Unobscured token</param>
        /// <returns> Obscured token</returns>
        private static string ObscureToken(string str)
        {
            var obscuredToken = str;
            var rand = new Random(DateTime.Now.Millisecond);

            for (var x = 1; x < str.Length * 2; x = x + 2)
            {
                if (x == 3 || x == 7)
                {
                    obscuredToken = obscuredToken.Insert(x, "|");
                }
                else
                {
                    obscuredToken = obscuredToken.Insert(x, rand.Next(0, 9).ToString());
                }
            }

            return obscuredToken;
        }

        /// <summary>
        /// Decodes a token
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Decoded token</returns>
        public JObject DecodeToken(string token)
        {
            if (token.Contains("%7C"))
                token = token.Replace("%7C", "|");

            var ip = DecodeIpAddress(token);
            var userId = DecodeUserId(token);

            return JObject.Parse("{'ip':"+ip+", 'id':"+userId+"}");
        }

        /// <summary>
        /// Converts the string to hex
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns>Converted string</returns>
        private static string StringToHex(string str)
        {
            var ipInt = Int64.Parse(str);
            return ipInt.ToString("X");
        }

        /// <summary>
        /// Removes unwanted chars from ip string
        /// </summary>
        /// <param name="ip">The raw ip address</param>
        /// <returns>IP without chars</returns>
        public string RemoveChars(string ip)
        {
            if (ip.Contains(":"))
            {
                ip = ip.Substring(0, ip.IndexOf(":"));
            }           
            ip = ip.Replace(".", string.Empty);

            return ip;
        }

        /// <summary>
        /// Decodes the IP address from a token
        /// </summary>
        /// <param name="token">Undecoded token</param>
        /// <returns>IP address</returns>
        private static string DecodeIpAddress(string token)
        {
            var end = token.LastIndexOf("|");
            var obscuredHexIp = token.Substring(0,end);
            var hexIp = "";
            for (var x = 0; x < obscuredHexIp.Length; x = x + 2)
            {
                hexIp += obscuredHexIp.Substring(x,1);
            }
            return DecodeHexToBase10(hexIp);
        }

        /// <summary>
        /// Decodes the user ID from a token
        /// </summary>
        /// <param name="token">Undecoded token</param>
        /// <returns>User ID</returns>
        private static int DecodeUserId(string token)
        {
            var start = token.LastIndexOf("|");
            var hexId = token.Substring(start+1);
            return DecodeHexToInt32(hexId);
        }

        /// <summary>
        /// Converts hex to int
        /// </summary>
        /// <param name="hex">Hex to decode</param>
        /// <returns>String converted from hex</returns>
        private static Int32 DecodeHexToInt32(string hex)
        {
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// Decodes hex to base 10
        /// Found at https://csharpfunctions.blogspot.com/2010/03/convert-hex-formatted-string-to-base64.html
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>Base 10 hex</returns>
        private static string DecodeHexToBase10(string hex)
        {
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber).ToString(); ;
        }       
    }
}
