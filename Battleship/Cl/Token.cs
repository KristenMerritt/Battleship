using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Battleship.Cl
{
    public class Token
    {
        private byte[] Time { get; set; }
        private int UserId { get; set; }
        private string UserIpAddress { get; set; }
        public string UserToken { get; set; }

        public Token(int userId, string ip)
        {
            UserId = userId;
            UserIpAddress = ip;

            //Console.WriteLine(DateTime.Now.ToString("G") + " -- Token User Ip Address: " + UserIpAddress);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- Token User Ip Address: " + UserIpAddress);

            Time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        }

        public Token()
        {
            Time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        }

        public string GenerateToken()
        {
            Debug.WriteLine(" ============= Generating New Token ==============");

            var removedChars = RemoveChars(UserIpAddress);
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- Removed Chars: " + removedChars);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- Removed Chars: " + removedChars);

            var hexIp = StringToHex(removedChars); // obscured ip address changed to HEX
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- HexIP: " + hexIp);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- HexIP: " + hexIp);

            var obscuredIp = ObscureToken(hexIp); // ip address with random numbers in between
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- Obscured IP: " + obscuredIp);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- Obscured IP: " + obscuredIp);

            var hexId = StringToHex(UserId.ToString()); // normal ID changed to HEX
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- Hex ID: " + hexId);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- Hex ID: " + hexId);

            UserToken = obscuredIp + "|" + hexId;
            return UserToken;           
        }

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

        public JObject DecodeToken(string token)
        {
            if (token.Contains("%7C"))
                token = token.Replace("%7C", "|");

            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (DecodeToken) Decoding token: " + token);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (DecodeToken) Decoding token: " + token);

            var ip = DecodeIpAddress(token);
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (DecodeToken) Decoded IP address: " + ip);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (DecodeToken) Decoded IP address: " + ip);

            var userId = DecodeUserId(token);
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (DecodeToken) Decoded user ID: " + userId);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (DecodeToken) Decoded user ID: " + userId);

            return JObject.Parse("{'ip':"+ip+", 'id':"+userId+"}");
        }

        private static string StringToHex(string str)
        {
            var ipInt = Int64.Parse(str);
            return ipInt.ToString("X");
        }

        public string RemoveChars(string ip)
        {
            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (RemoveChars) IP given to RemoveChars(): " + ip);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (RemoveChars) IP given to RemoveChars(): " + ip);
            if (ip.Contains(":"))
            {
                ip = ip.Substring(0, ip.IndexOf(":"));
            }           
            ip = ip.Replace(".", string.Empty);

            //Console.WriteLine(DateTime.Now.ToString("G") + " -- (RemoveChars) IP returned from RemoveChars(): " + ip);
            //Debug.WriteLine(DateTime.Now.ToString("G") + " -- (RemoveChars) IP returned from RemoveChars(): " + ip);

            return ip;
        }

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

        private static int DecodeUserId(string token)
        {
            var start = token.LastIndexOf("|");
            var hexId = token.Substring(start+1);
            return DecodeHexToInt32(hexId);
        }

        private static Int32 DecodeHexToInt32(string hex)
        {
            return Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        // https://csharpfunctions.blogspot.com/2010/03/convert-hex-formatted-string-to-base64.html
        private static string DecodeHexToBase10(string hex)
        {
            return int.Parse(hex, System.Globalization.NumberStyles.HexNumber).ToString(); ;
        }       
    }
}
