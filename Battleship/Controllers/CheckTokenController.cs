using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/CheckToken")]
    public class CheckTokenController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class

        /// <summary>
        /// Controller that is used for checking tokens.
        /// </summary>
        /// <param name="playerRepo"></param>
        public CheckTokenController(PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
        }

        /// <summary>
        /// Validates a token provided.
        /// GET: api/CheckToken/{token}
        /// </summary>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        [HttpGet]
        [Route("{token}")]
        public bool ValidateToken(string token)
        {
            return base.ValidateToken(token);
        }
    }
}