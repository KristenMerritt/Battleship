using Battleship.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.Controllers
{
    [Produces("application/json")]
    [Route("api/CheckToken")]
    public class CheckTokenController : BaseController
    {
        private readonly PlayerRepo _playerRepo; // DB repo class

        public CheckTokenController(PlayerRepo playerRepo) : base(playerRepo)
        {
            _playerRepo = playerRepo;
        }

        [HttpGet]
        [Route("{token}")]
        public bool ValidateToken(string token)
        {
            return base.ValidateToken(token);
        }
    }
}